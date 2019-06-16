using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Tecnu.Core.Record;
using XNuvem.Data;
using XNuvem.UI.DataTable;

namespace Tecnu.Controllers
{
    public class TeacherController : Controller
    {
        private readonly IJDataTable<TeacherRecord> _jdtTeacherDataTable;
        private readonly IRepository<TeacherRecord> _record;
        public TeacherController(IJDataTable<TeacherRecord> jDataTable, IRepository<TeacherRecord> repository)
        {
            _jdtTeacherDataTable = jDataTable;
            _record = repository;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            return await Task.FromResult(View());
        }

        public async Task<ActionResult> GetTeacher(FormCollection values)
        {
            _jdtTeacherDataTable.SetParameters(values);
            var result = await Task.Run<DataTableResult>(() => _jdtTeacherDataTable.Execute(
                q => q.Select(s => new
                {
                    s.Id,
                    Nome = s.Name,
                    Telefone = s.Telephone,
                    DataNascimento = s.DateBirth.ToShortDateString()
                })));
            return Json(result);
        }
    }
}