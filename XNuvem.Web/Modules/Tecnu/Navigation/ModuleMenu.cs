using XNuvem.UI.Navigation;

namespace Tecnu.Navigation
{
    public class ModuleMenu : IMenuProvider
    {
        public void BuildMenu(MenuBuilder builder)
        {
            builder.AddGroup("2", "Alunos");
            builder.AddAction("2.1", "Listar Alunos", "Index", "Student", new { area = "Tecnu" });
            builder.AddAction("2.2", "Inserir Alunos", "StudentEdit", "Student", new { area = "Tecnu" });
            builder.AddGroup("3", "Professor");
            builder.AddAction("3.1", "Listar Professores", "Index", "Teacher", new { area = "Tecnu" });
            builder.AddAction("3.2", "Inserir Alunos", "Add", "Teacher", new { area = "Tecnu" });
            builder.AddGroup("4", "Turmas");
            builder.AddAction("4.1", "Listar Turmas", "Index", "Class", new { area = "Tecnu" });
            builder.AddAction("4.2", "Inserir Turmas", "Add", "Class", new { area = "Tecnu" });
        }
    }
}