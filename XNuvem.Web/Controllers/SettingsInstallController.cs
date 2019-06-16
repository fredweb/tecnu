using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Tecnu.Core.Record;
using XNuvem.Data;
using XNuvem.Data.Schema;
using XNuvem.Environment;
using XNuvem.Environment.Configuration;
using XNuvem.Logging;
using XNuvem.Security;
using XNuvem.Web.Models;

namespace XNuvem.Web.Controllers
{
    public class SettingsInstallController : Controller
    {
        private readonly IShellSettingsManager _shellSettingsManager;
        private readonly ISchemaUpdate _schemaUpdate;
        private readonly ITransactionManager _transactionManager;
        private readonly IServiceContext _serviceContext;
        private readonly IRepository<PeriodRecord> _periodRecord;
        ILogger Logger { get; set; }

        public SettingsInstallController(
            IShellSettingsManager shellSettingsManager,
            ISchemaUpdate schemaUpdate,
            ITransactionManager transactionManager,
            IServiceContext serviceContext,
            IRepository<PeriodRecord> periodRecord)
        {
            _shellSettingsManager = shellSettingsManager;
            _schemaUpdate = schemaUpdate;
            _transactionManager = transactionManager;
            _serviceContext = serviceContext;
            Logger = NullLogger.Instance;
            _periodRecord = periodRecord;
        }

        [HttpGet]
        public ActionResult Install()
        {
            if (!_schemaUpdate.HasUpdates())
            {
                Logger.Information("Has no updates to install.");
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Install(InstallViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!_schemaUpdate.HasUpdates())
                {
                    Logger.Information("Has no updates to install.");
                    return RedirectToAction("Login", "Account");
                }
                var settings = _shellSettingsManager.GetSettings();
                settings.ConnectionSettings.DataConnectionString = model.ConnectionString;
                settings.ConnectionSettings.DataProvider = "System.Data.SqlClient";
                _shellSettingsManager.StoreSettings(settings);
                _schemaUpdate.CreateDatabase();

                // Restart transaction and create a new service to context
                _transactionManager.RequireNew();
                var userService = _serviceContext.Resolve<IUserService>();
                var user = new User()
                {
                    UserName = model.UserName,
                    Email = model.Email
                };
                userService.Create(user, model.Password);
                userService.UserManager.AddClaim(user.Id, DefaultClaims.SiteUser);
                userService.UserManager.AddClaim(user.Id, DefaultClaims.Administrator);
                var signInService = _serviceContext.Resolve<ISignInService>();
                var result = await signInService.PasswordSignInAsync(user.UserName, model.Password, isPersistent: false, shouldLockout: false);
                _transactionManager.RequireNew();
                InitialCharge();
                if (result == SignInStatus.Success)
                {
                    return RedirectToAction("Index", "Home", new { area = "" });
                }
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Update()
        {
            var model = new UpdateViewModel();
            if (_schemaUpdate.HasUpdates())
            {
                model.ReadyToUpdate = true;
                model.Title = "O Sistema necessita ser atualizado";
                model.Message = "É necessário atualizar o sistema para que ele funcione corretamente. Clique em continuar para atualizar o sistema";
            }
            else
            {
                model.ReadyToUpdate = false;
                model.Title = "Sistema atualizado";
                model.Message = "Não é necessário atualizar o sistema. O sistema já se encontra atualizado";
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Update(UpdateViewModel model)
        {
            if (_schemaUpdate.HasUpdates())
            {
                _schemaUpdate.UpdateDatabse();
            }
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        [NonAction]
        private void InitialCharge()
        {
            #region Period
            if (!_periodRecord.Table.Any(w => w.Name.Equals("Manhã")))
            {
                var period = new PeriodRecord
                {
                    Name = "Manhã",
                    Update = DateTime.UtcNow,
                    When = DateTime.UtcNow
                };
                _periodRecord.Create(period);
            }

            if (!_periodRecord.Table.Any(w => w.Name.Equals("Tarde")))
            {
                var period = new PeriodRecord
                {
                    Name = "Tarde",
                    Update = DateTime.UtcNow,
                    When = DateTime.UtcNow
                };
                _periodRecord.Create(period);
            }

            if (!_periodRecord.Table.Any(w => w.Name.Equals("Noite")))
            {
                var period = new PeriodRecord
                {
                    Name = "noite",
                    Update = DateTime.UtcNow,
                    When = DateTime.UtcNow
                };
                _periodRecord.Create(period);
            }
            #endregion
        }
    }
}