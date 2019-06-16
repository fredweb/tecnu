using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XNuvem.UI.Navigation;

namespace XNuvem.Tests.UI
{
    [TestFixture]
    public class MenuModuleTest
    {
        [Test]
        public void MenuPosition() {
            var entry = new MenuEntry() {
                Position = "12.03"
            };
            int level = entry.Level;
            Assert.That(level, Is.EqualTo(03));
            string father = entry.Father;
            Assert.That(father, Is.EqualTo("12"));

        }

        [Test]
        public void BuildMenu() {
            var build = new MenuBuilder();
            build.Add(new MenuEntry() {
                Position = "1",
                ActionName = "Add",
                ControllerName = "Test"
            });
            build.Add(new MenuEntry() {
                Position = "2",
                ActionName = "Add",
                ControllerName = "Test 2"
            });
            build.Add(new MenuEntry() {
                Position = "1.02",
                ActionName = "Add SubMenu",
                ControllerName = "Test"
            });
            Console.WriteLine("Root contains {0} menus.", build.RootMenu.Submenu.Count);
            Console.WriteLine();
            foreach (var m in build.RootMenu.Submenu) {
                Console.WriteLine("Root {0} - {1}", m.Position, m.ActionName);
                foreach (var s in m.Submenu) {
                    Console.WriteLine("    -> Submenu {0} - {1}", s.Position, s.ActionName);
                }
            }
            Assert.That(build.RootMenu.Submenu.Count, Is.EqualTo(2));
        }

        [Test]
        public void TransverseMenu() {
            var build = new MenuBuilder();
            build.Add(new MenuEntry() {
                Position = "1",
                ActionName = "Add",
                ControllerName = "Test"
            });
            build.Add(new MenuEntry() {
                Position = "2",
                ActionName = "Add",
                ControllerName = "Test 2"
            });
            build.Add(new MenuEntry() {
                Position = "1.02",
                ActionName = "Add SubMenu",
                ControllerName = "Test"
            });
            build.Add(new MenuEntry() {
                Position = "1.03",
                ActionName = "Add SubMenu",
                ControllerName = "Test"
            });
            build.Add(new MenuEntry() {
                Position = "1.01",
                ActionName = "Add SubMenu",
                ControllerName = "Test"
            });

            foreach (var m in build.Build().Transverse()) {
                Console.WriteLine("Menu -> {0} - {1}", m.Position, m.ActionName);
            }
            Assert.That(build.RootMenu.Transverse(true).Count(), Is.EqualTo(6));
        }

        [Test]
        public void PerformanceTest() {
            var builder = new MenuBuilder();
            for (int i = 1; i <= 10; i++) {
                var m = new MenuEntry() {
                    Position = i.ToString(),
                    ActionName = string.Format("Menu {0}", i)
                };
                builder.Add(m);

                for (int j = 100; j >= 1; j--) {
                    var sm = new MenuEntry() {
                        Position = i.ToString() + "." + j.ToString(),
                        ActionName = string.Format("Menu {0} - {1}", i, j)
                    };
                    builder.Add(sm);
                }
            }
            int index = 0;
            foreach (var m in builder.Build().Transverse(false)) {
                Console.WriteLine("{2} -> {0} - {1}", m.Position, m.ActionName, index++);
            }
        }
    }
}
