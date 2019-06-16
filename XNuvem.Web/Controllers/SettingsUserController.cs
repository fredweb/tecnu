using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using XNuvem.Mvc;
using XNuvem.Security;
using XNuvem.UI.DataTable;
using XNuvem.UI.Model;
using XNuvem.Web.Models;

namespace XNuvem.Web.Controllers
{
    [Authorize]
    [Admin("", "security")]
    public class SettingsUserController : XNuvemController
    {
        private readonly IJDataTable<User> _jdtUsers;
        private readonly IUserService _userService;
        private readonly ISignInService _signInService;

        public SettingsUserController(
            IJDataTable<User> jdtUsers, 
            IUserService userService,
            IAuthenticationManager authenticationManager,
            ISignInService signInService) {
            _jdtUsers = jdtUsers;
            _jdtUsers.SearchOn("UserName");
            _jdtUsers.SearchOn("FullName");
            _userService = userService;
            _signInService = signInService;
        }

        [HttpGet]
        public async Task<ActionResult> UserList() {
            return await Task.FromResult(View());
        }

        [HttpPost]
        public async Task<ActionResult> UserList(FormCollection values) {
            _jdtUsers.SetParameters(values);
            var result = await Task.Run<DataTableResult>(() => _jdtUsers.Execute(
                q => q.Select(s => new {
                    Id = s.Id,
                    UserName = s.UserName,
                    FullName = s.FullName,
                    Email = s.Email
                })));
            return Json(result);
        }

        [HttpGet]
        public async Task<ActionResult> UserEdit(string id) {
            if (!string.IsNullOrEmpty(id)) {
                var usr = await _userService.UserManager.FindByIdAsync(id);
                var model = new UserEditViewModel {
                    Id = usr.Id,
                    UserName = usr.UserName,
                    FullName = usr.FullName,
                    Email = usr.Email,
                    SlpCode = usr.SlpCode,
                    SlpName = usr.SlpName,
                    QryGroup60 = usr.QryGroup60.Equals("Y", StringComparison.InvariantCultureIgnoreCase),
                    QryGroup61 = usr.QryGroup61.Equals("Y", StringComparison.InvariantCultureIgnoreCase),
                    QryGroup62 = usr.QryGroup62.Equals("Y", StringComparison.InvariantCultureIgnoreCase)
                };
                return View(model);
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> UserEdit(UserEditViewModel model) {
            if (ModelState.IsValid) {
                if (!string.IsNullOrEmpty(model.Id)) { // Update user
                    var entity = await _userService.UserManager.FindByIdAsync(model.Id);
                    entity.UserName = model.UserName;
                    entity.FullName = model.FullName;
                    entity.Email = model.Email;
                    entity.SlpCode = model.SlpCode;
                    entity.SlpName = model.SlpName;
                    entity.QryGroup60 = model.QryGroup60 ? "Y" : "N";
                    entity.QryGroup61 = model.QryGroup61 ? "Y" : "N";
                    entity.QryGroup62 = model.QryGroup62 ? "Y" : "N";
                    _userService.Update(entity);
                    if (!string.IsNullOrWhiteSpace(model.Password)) {
                        await _userService.ForceChangePasswordAsync(entity, model.Password);
                    }
                }
                else { // Create new user
                    var checkUser = _userService.FindByName(model.UserName);
                    if (checkUser != null) {
                        throw new XNuvemCoreException(string.Format("O login de usuário {0} já existe.", model.UserName));
                    }
                    if (string.IsNullOrWhiteSpace(model.Password)) {
                        throw new XNuvemCoreException("É necessário informar a senha para criar um novo usuário.");
                    }
                    var entity = new User {
                        UserName = model.UserName,
                        FullName = model.FullName,
                        Email = model.Email,
                        SlpCode = model.SlpCode,
                        SlpName = model.SlpName,
                        QryGroup60 = model.QryGroup60 ? "Y":"N",
                        QryGroup61 = model.QryGroup61 ? "Y" : "N",
                        QryGroup62 = model.QryGroup62 ? "Y" : "N"
                    };
                    _userService.Create(entity, model.Password);
                }
                return ViewOrAjax(model);            
            }
            if (Request.IsAjaxRequest()) {
                return AjaxError("Erro ao adicionar. Verifique se as informações estão digitadas corretamente.");
            }
            else {
                return ViewOrAjax(model);
            }
        }

        [HttpGet]
        public async Task<ActionResult> ChangePassword() {
            return await Task.FromResult(View());
        }

        [HttpPost]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model) {
            var currentUser = ControllerContext.HttpContext.User;
            if (ModelState.IsValid && currentUser.Identity.IsAuthenticated) {
                var user = await _userService.UserManager.FindByNameAsync(currentUser.Identity.Name);
                if (user == null) {
                    if (Request.IsAjaxRequest()) {
                        return AjaxError("Usuário não encontrado ou excluído.");
                    }
                    else {
                        ModelState.AddModelError("", "Usuário não encontrado ou foi excluído.");
                        return View(model);
                    }
                }
                var changeResult = await _userService.UserManager.ChangePasswordAsync(user.Id, model.OldPassword, model.Password);
                if (!changeResult.Succeeded) {
                    if (Request.IsAjaxRequest()) {
                        return AjaxError("Não foi possível alterar a senha do usuário. Verifique se as informações estão corretas.");
                    }
                    else {
                        ModelState.AddModelError("", "Não foi possível alterar a senha do usuário. Verifique se as informações estão corretas.");
                        return View(model);
                    }
                }

                // Refresh authentication token
                _signInService.SignOut();
                var result = await _signInService.PasswordSignInAsync(user.UserName, model.Password, false, false);
                if (result != SignInStatus.Success) {
                    if (Request.IsAjaxRequest()) {
                        return AjaxError("Não foi possível autenticar o usuário.");
                    }
                    else {
                        ModelState.AddModelError("", "Não foi possível autenticar o usuário.");
                        return View(model);
                    }
                }                
                
                // FIX: Este retorno do controller é necessário melhorar
                return ViewOrAjax(model);
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> UserDelete(string keys) {
            if (string.IsNullOrEmpty(keys)) {
                return Json(new MessageResult {
                    IsError = true,
                    Messages = new[] { "Nenhuma chave passada para o usuário." }
                });
            }
            var idsMap = keys.Split(';');
            foreach (var id in idsMap) {
                var user = await _userService.UserManager.FindByIdAsync(id);
                if(user == null) {
                    return Json(new MessageResult {
                    IsError = true,
                    Messages = new[] { string.Format("Chave de usuário {0} não encontrada.", id) }});
                }
                await _userService.UserManager.DeleteAsync(user);
            }
            return Json(new MessageResult {
                IsError = false,
                Messages = new[] { "Operação concluída com êxito." }
            });
        }

    }
}