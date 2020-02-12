using EmployeeProjectManagement.Datatable;
using EmployeeProjectManagement.Models;
using EmployeeProjectManagement.Models.Data;
using EmployeeProjectManagement.Models.ViewModels.Manager;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EmployeeProjectManagement.Controllers
{
    public class ManagerController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();

        public ManagerController()
        {

        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Projects()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> AddProject(ProjectViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.Id != null)
                    {
                        var project = await _context.Projects.Where(p => p.Id == model.Id).FirstOrDefaultAsync();

                        if (project != null)
                        {
                            project.Cost = model.Cost;
                            project.Name = model.Name;

                            await _context.SaveChangesAsync(HttpContext.User.Identity.Name);
                            return Json(new { Success = true });
                        }
                        else
                        {
                            return Json(new { Error = true });
                        }
                    }
                    else
                    {
                        var project = new Project()
                        {
                            Cost = model.Cost,
                            Name = model.Name
                        };

                        _context.Projects.Add(project);
                        await _context.SaveChangesAsync(HttpContext.User.Identity.Name);
                        return Json(new { Success = true });
                    }
                }
                return Json(new { Error = true });
            }
            catch (Exception ex)
            {
                return Json(new { Error = true });
            }
        }

        [HttpPost]
        public async Task<JsonResult> DeleteProject(int? Id)
        {
            try
            {
                var project = await _context.Projects.Where(p => p.Id == Id).FirstOrDefaultAsync();

                if (project != null)
                {
                    var mappings = await _context.EmployeeToProjects.Where(e => e.ProjectId == Id).ToListAsync();

                    if (mappings != null)
                    {
                        _context.EmployeeToProjects.RemoveRange(mappings);
                    }

                    _context.Projects.Remove(project);
                    await _context.SaveChangesAsync(HttpContext.User.Identity.Name);

                    return Json(new { Success = true });
                }
                return Json(new { Error = true });
            }
            catch (Exception ex)
            {
                return Json(new { Error = true });
            }
        }

        public async Task<ActionResult> Employees(int projid)
        {
            var project = await _context.Projects.Where(p => p.Id == projid).FirstOrDefaultAsync();

            if (project != null)
            {
                ViewBag.ProjectId = project.Id;
                ViewBag.ProjectName = project.Name;

                return View();
            }
            return RedirectToAction(nameof(ManagerController.Projects), "Manager");
        }

        [HttpPost]
        public async Task<JsonResult> AddEmployee(EmployeeDetailsViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.EmployeeId != null)
                    {
                        var employee = await _context.Employees.Where(p => p.Id == model.EmployeeId).FirstOrDefaultAsync();

                        if (employee != null)
                        {
                            employee.FirstName = model.FirstName;
                            employee.LastName = model.LastName;
                            employee.JoiningDate = DateTime.ParseExact(model.JoiningDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                            await _context.SaveChangesAsync(HttpContext.User.Identity.Name);
                            return Json(new { Success = true });
                        }
                        else
                        {
                            return Json(new { Error = true });
                        }
                    }
                    else
                    {
                        var project = await _context.Projects.Where(p => p.Id == model.ProjectId).FirstOrDefaultAsync();
                        if (project != null)
                        {
                            var employee = new Employee
                            {
                                FirstName = model.FirstName,
                                LastName = model.LastName,
                                JoiningDate = DateTime.ParseExact(model.JoiningDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)
                            };

                            _context.Employees.Add(employee);
                            await _context.SaveChangesAsync(HttpContext.User.Identity.Name);

                            var empProjectMapping = new EmployeeToProject
                            {
                                ProjectId = project.Id,
                                EmployeeId = employee.Id
                            };

                            _context.EmployeeToProjects.Add(empProjectMapping);
                            await _context.SaveChangesAsync(HttpContext.User.Identity.Name);

                            return Json(new { Success = true });
                        }
                        return Json(new { Error = true });
                    }
                }
                return Json(new { Error = true });
            }
            catch (Exception ex)
            {
                return Json(new { Error = true });
            }
        }

        [HttpPost]
        public async Task<JsonResult> DeleteEmployee(int? Id)
        {
            try
            {
                var employee = await _context.Employees.Where(p => p.Id == Id).FirstOrDefaultAsync();

                if (employee != null)
                {
                    var mappings = await _context.EmployeeToProjects.Where(e => e.EmployeeId == Id).ToListAsync();

                    if (mappings != null)
                    {
                        _context.EmployeeToProjects.RemoveRange(mappings);
                    }

                    _context.Employees.Remove(employee);
                    await _context.SaveChangesAsync(HttpContext.User.Identity.Name);

                    return Json(new { Success = true });
                }
                return Json(new { Error = true });
            }
            catch (Exception ex)
            {
                return Json(new { Error = true });
            }
        }

        [HttpPost]
        public JsonResult GetEmployees(DataTableAjaxPostModel model)
        {
            var searchBy = (model.search != null) ? model.search.value : null;
            var take = model.length;
            var skip = model.start;

            string sortBy = "";
            bool sortDir = true;

            if (model.order != null)
            {
                sortBy = model.columns[model.order[0].column].data;
                sortDir = model.order[0].dir.ToLower() == "asc";
            }

            if (string.IsNullOrEmpty(model.startdate))
            {
                model.startdate = "01/01/1753";
            }
            if (string.IsNullOrEmpty(model.enddate))
            {
                model.enddate = "01/01/9999";
            }

            DateTime dtStart = DateTime.ParseExact(model.startdate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime dtEnd = DateTime.ParseExact(model.enddate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            var projectsEmployees = new List<ProjectEmployeeDetailsViewModel>();

            if (!string.IsNullOrEmpty(searchBy))
            {
                projectsEmployees = _context.Database.SqlQuery<ProjectEmployeeDetailsViewModel>("select * from GetEmployeesWithProjects(@skip,@count,@orderby,@searchby,@startDate,@endDate)", new SqlParameter("skip", skip), new SqlParameter("count", take), new SqlParameter("orderby", sortBy + " " + (sortDir ? "asc" : "desc")), new SqlParameter("searchby", "%" + searchBy + "%"), new SqlParameter("startDate",dtStart), new SqlParameter("endDate", dtEnd)).ToList();
                var filteredResultsCount = _context.Database.SqlQuery<int>("select * from GetProjectsEmployeesCount");

                return Json(new
                {
                    draw = model.draw,
                    recordsFiltered = filteredResultsCount,
                    recordsTotal = filteredResultsCount,
                    data = projectsEmployees
                }, JsonRequestBehavior.AllowGet);
            }

            var totalAssigned = _context.Database.SqlQuery<int>("select * from GetProjectsEmployeesCount");
            if (skip < 0)
            {
                projectsEmployees = _context.Database.SqlQuery<ProjectEmployeeDetailsViewModel>("select * from GetEmployeesWithProjects(@skip,@count,@orderby,@startDate,@endDate)", new SqlParameter("skip", 0), new SqlParameter("count", take), new SqlParameter("orderby", sortBy + " " + (sortDir ? "asc" : "desc")), new SqlParameter("searchby", "%%"), new SqlParameter("startDate", dtStart), new SqlParameter("endDate", dtEnd)).ToList();
                return Json(new
                {
                    draw = model.draw,
                    recordsFiltered = totalAssigned,
                    recordsTotal = totalAssigned,
                    data = projectsEmployees
                }, JsonRequestBehavior.AllowGet);
            }

            projectsEmployees = _context.Database.SqlQuery<ProjectEmployeeDetailsViewModel>("select * from GetEmployeesWithProjects(@skip,@count,@orderby,@searchby,@startDate,@endDate)", new SqlParameter("skip", skip), new SqlParameter("count", take), new SqlParameter("orderby", sortBy + " " + (sortDir ? "asc" : "desc")), new SqlParameter("searchby", "%%"), new SqlParameter("startDate", dtStart), new SqlParameter("endDate", dtEnd)).ToList();
            return Json(new
            {
                draw = model.draw,
                recordsFiltered = totalAssigned,
                recordsTotal = totalAssigned,
                data = projectsEmployees
            }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetEmployeesByProject(DataTableAjaxPostModel model)
        {
            var searchBy = (model.search != null) ? model.search.value : null;
            var take = model.length;
            var skip = model.start;

            string sortBy = "";
            bool sortDir = true;

            if (model.order != null)
            {
                sortBy = model.columns[model.order[0].column].data;
                sortDir = model.order[0].dir.ToLower() == "asc";
            }

            var totalEmployees = await _context.EmployeeToProjects.Include(e => e.Project).Include(e => e.Employee).Where(e => e.Project != null && e.ProjectId == model.ProjectId && e.Employee != null).CountAsync();

            if (!string.IsNullOrEmpty(searchBy))
            {
                var employees = (await _context.EmployeeToProjects.Include(e => e.Project).Include(e => e.Employee).Where(e => e.Project != null && e.ProjectId == model.ProjectId && e.Employee != null).OrderBy(p => p.Employee.FirstName).Skip(skip).Take(take).Select(e => e.Employee).ToListAsync()).Select(e => new { Id = e.Id, FirstName = e.FirstName, LastName = e.LastName, JoiningDate = e.JoiningDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) });
                return Json(new
                {
                    draw = model.draw,
                    recordsFiltered = totalEmployees,
                    recordsTotal = totalEmployees,
                    data = employees
                }, JsonRequestBehavior.AllowGet);
            }

            if (skip < 0)
            {
                var employees = (await _context.EmployeeToProjects.Include(e => e.Project).Include(e => e.Employee).Where(e => e.Project != null && e.ProjectId == model.ProjectId && e.Employee != null).OrderBy(p => p.Employee.FirstName).Skip(0).Take(take).Select(e => e.Employee).ToListAsync()).Select(e => new { Id = e.Id, FirstName = e.FirstName, LastName = e.LastName, JoiningDate = e.JoiningDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) });
                return Json(new
                {
                    draw = model.draw,
                    recordsFiltered = totalEmployees,
                    recordsTotal = totalEmployees,
                    data = employees
                }, JsonRequestBehavior.AllowGet);
            }

            var employeesTotal = (await _context.EmployeeToProjects.Include(e => e.Project).Include(e => e.Employee).Where(e => e.Project != null && e.ProjectId == model.ProjectId && e.Employee != null).OrderBy(p => p.Employee.FirstName).Skip(skip).Take(take).Select(e => e.Employee).ToListAsync()).Select(e => new { Id = e.Id, FirstName = e.FirstName, LastName = e.LastName, JoiningDate = e.JoiningDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) });

            return Json(new
            {
                draw = model.draw,
                recordsFiltered = totalEmployees,
                recordsTotal = totalEmployees,
                data = employeesTotal
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> GetProjects(DataTableAjaxPostModel model)
        {
            var searchBy = (model.search != null) ? model.search.value : null;
            var take = model.length;
            var skip = model.start;

            string sortBy = "";
            bool sortDir = true;

            if (model.order != null)
            {
                sortBy = model.columns[model.order[0].column].data;
                sortDir = model.order[0].dir.ToLower() == "asc";
            }

            var totalProjects = await _context.Projects.CountAsync();
            var projects = new List<Project>();

            if (!string.IsNullOrEmpty(searchBy))
            {
                projects = await _context.Projects.Where(p => p.Name.Contains(searchBy)).OrderBy(p => p.Name).Skip(skip).Take(take).ToListAsync();
                return Json(new
                {
                    draw = model.draw,
                    recordsFiltered = totalProjects,
                    recordsTotal = totalProjects,
                    data = projects
                }, JsonRequestBehavior.AllowGet);
            }

            if (skip < 0)
            {
                projects = await _context.Projects.OrderBy(p => p.Name).Skip(0).Take(take).ToListAsync();
                return Json(new
                {
                    draw = model.draw,
                    recordsFiltered = totalProjects,
                    recordsTotal = totalProjects,
                    data = projects
                }, JsonRequestBehavior.AllowGet);
            }
            projects = await _context.Projects.OrderBy(p => p.Name).Skip(skip).Take(take).ToListAsync();

            return Json(new
            {
                draw = model.draw,
                recordsFiltered = totalProjects,
                recordsTotal = totalProjects,
                data = projects
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> ProjectEmployeeDetails(ProjectEmployeeDetailsViewModel model)
        {
            try
            {
                if (model.EmployeeId != null && model.ProjectId != null)
                {
                    var project = await _context.Projects.Where(p => p.Id == model.ProjectId).FirstOrDefaultAsync();
                    var employee = await _context.Employees.Where(e => e.Id == model.EmployeeId).FirstOrDefaultAsync();

                    if (project != null && employee != null)
                    {
                        var projectMapping = await _context.EmployeeToProjects.Include(e => e.Employee).Include(e => e.Project).Where(e => e.EmployeeId == employee.Id && e.ProjectId == project.Id).FirstOrDefaultAsync();
                        if (projectMapping != null)
                        {
                            projectMapping.Project.Name = model.ProjectName;
                            projectMapping.Project.Cost = model.Cost;

                            projectMapping.Employee.FirstName = model.FirstName;
                            projectMapping.Employee.LastName = model.LastName;
                            if (!string.IsNullOrEmpty(model.JoiningDate))
                            {
                                projectMapping.Employee.JoiningDate = DateTime.ParseExact(model.JoiningDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            }

                            await _context.SaveChangesAsync(HttpContext.User.Identity.Name);
                            return Json(new { Success = true });
                        }
                        else
                        {
                            return Json(new { Error = true });
                        }
                    }
                    else
                    {
                        return Json(new { Error = true });
                    }
                }
                return Json(new { Error = true });
            }
            catch (Exception ex)
            {
                return Json(new { Error = true });
            }
        }

        [HttpPost]
        public async Task<JsonResult> DeleteProjectEmployeeDetails(int? ProjectId, int? EmployeeId)
        {
            try
            {
                if (ProjectId != null && EmployeeId != null)
                {
                    var project = await _context.Projects.Where(p => p.Id == ProjectId).FirstOrDefaultAsync();
                    var employee = await _context.Employees.Where(e => e.Id == EmployeeId).FirstOrDefaultAsync();

                    if (project != null && employee != null)
                    {
                        var projectMapping = await _context.EmployeeToProjects.Include(e => e.Employee).Include(e => e.Project).Where(e => e.EmployeeId == employee.Id && e.ProjectId == project.Id).FirstOrDefaultAsync();
                        if (projectMapping != null)
                        {
                            _context.EmployeeToProjects.Remove(projectMapping);
                            await _context.SaveChangesAsync(HttpContext.User.Identity.Name);
                        }
                        return Json(new { Success = true });
                    }
                    return Json(new { Error = true });
                }
                return Json(new { Error = true });
            }
            catch (Exception ex)
            {
                return Json(new { Error = true });
            }
        }
    }
}