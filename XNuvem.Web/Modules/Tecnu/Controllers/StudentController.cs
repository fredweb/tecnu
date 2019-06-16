using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Tecnu.Core.Record;
using Tecnu.Core.ViewModel;
using Tecnu.Resource;
using XNuvem.Data;
using XNuvem.UI.DataTable;
using XNuvem.UI.Model;

namespace Tecnu.Controllers
{
    public class StudentController : Controller
    {
        private readonly IJDataTable<StudentRecord> _jdtStudenteDataTable;
        private readonly IRepository<StudentRecord> _record;

        public StudentController(IJDataTable<StudentRecord> jdtStudenteDataTable, IRepository<StudentRecord> record)
        {
            _jdtStudenteDataTable = jdtStudenteDataTable;
            _record = record;

        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<ActionResult> StudentEdit(int? id)
        {
            if (id.HasValue)
            {
                var record = _record.Get(id.Value);
                if (record != null)
                {
                    var model = new StudentViewModel
                    {
                        Id = id.Value,
                        Cpf = record.Cpf.ToString(@"00.000.000-00"),
                        DateBirth = record.DateBirth.ToShortDateString(),
                        AdditionalInformation = record.AdditionalInformation,
                        Email = record.Email,
                        Name = record.Name,
                        Telephone = record.Telephone.ToString()
                    };
                    return View(model);
                }
            }
            return await Task.FromResult(View());
        }

        [HttpPost]
        public async Task<ActionResult> StudentEdit(StudentViewModel model)
        {
            try
            {
                var record = new StudentRecord
                {
                    Id = model.Id ?? 0,
                    Telephone = Convert.ToInt64(model.Telephone),
                    AdditionalInformation = model.AdditionalInformation,
                    Cpf = Convert.ToInt64(model.Cpf),
                    DateBirth = Convert.ToDateTime(model.DateBirth),
                    Email = model.Email,
                    Name = model.Name,
                    Update = DateTime.UtcNow
                };

                if (record.Id == 0)
                    record.When = DateTime.UtcNow;

                if (record.Id == 0)
                    _record.Create(record);
                else
                    _record.Update(record);

                return Json(new MessageResult
                {
                    IsError = false,
                    Messages = new[] { Mensagens.MSG_OPERACAO_SUCESSO }
                });
            }
            catch
            {
                return Json(new MessageResult
                {
                    IsError = true,
                    Messages = new[] { Mensagens.MSG_ERRO_DEFAULT }
                });
            }
        }


        [HttpDelete]
        public async Task<ActionResult> StudentDelete(string keys)
        {
            if (string.IsNullOrEmpty(keys))
            {
                return Json(new MessageResult
                {
                    IsError = true,
                    Messages = new[] { "Nenhuma chave passada para o usuário." }
                });
            }

            await Task.Run(() =>
            {
                var idsMap = keys.Split(';');
                foreach (var id in idsMap)
                {
                    int auxId;
                    if (int.TryParse(id, out auxId))
                    {
                        var curso = _record.Get(auxId);
                        if (curso != null)
                            _record.Delete(curso);
                    }
                }
            });
            return Json(new MessageResult
            {
                IsError = false,
                Messages = new[] { Mensagens.MSG_OPERACAO_SUCESSO }
            });
        }

        [HttpPost]
        public async Task<ActionResult> StudentList(FormCollection values)
        {
            _jdtStudenteDataTable.SetParameters(values);
            var result = await Task.Run<DataTableResult>(() => _jdtStudenteDataTable.Execute(
                q => q.Select(s => new
                {
                    s.Id,
                    Nome = s.Name,
                    s.Email,
                    Telefone = s.Telephone,
                    DataNascimento = s.DateBirth.ToShortDateString()
                })));
            return Json(result);
        }
    }
}