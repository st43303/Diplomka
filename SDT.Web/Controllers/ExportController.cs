using SDT.Web.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace SDT.Web.Controllers
{
    [OutputCache(VaryByParam = "*", Duration = 0, NoStore = true)]
    public class ExportController : Controller
    {
        private SDTEntities db = new SDTEntities();
        private Document document;
        private PdfWriter pdfWriter;


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        [HttpPost]
        public FileResult Index(bool Team, bool Requirement, bool UseCase, bool Scenario, bool Actors, bool Tasks, int projectID)
        {
            int userID = (int)Session["userID"];
            var user = db.Users.Find(userID);
            var project = db.Projects.Find(projectID);

            try
            {
                MemoryStream workStream = new MemoryStream();
                StringBuilder status = new StringBuilder("");
                string fileName = "Dokumentace " + DateTime.Now.ToShortDateString() + ".pdf";
                document = new Document();
                document.SetMargins(60, 60, 60, 60);


                //string strAttachment = Server.MapPath("~/Downloads/" + fileName);
                //string strAttachment = Server.MapPath(fileName);
                pdfWriter = PdfWriter.GetInstance(document, workStream);
                pdfWriter.CloseStream = false;
                PageEventHelper pageEvent = new PageEventHelper();
                pdfWriter.PageEvent = pageEvent;
                document.Open();

                document.AddAuthor(user.Name + " " + user.Surname);
                document.AddCreationDate();
                document.AddTitle("Projekt " + project.Name + " - dokumentace");

                document.Add(titlePage(projectID));

                if (Team)
                {
                    document.NewPage();
                    document.Add(tableTeam(projectID));
                }

                if (Tasks)
                {
                    document.NewPage();
                    document.Add(tableTasks(projectID));
                }

                if (Requirement)
                {
                    document.NewPage();
                    document.Add(tableRequirements(projectID));

                }

                if (Actors)
                {
                    document.NewPage();
                    document.Add(tableActors(projectID));
                }

                if (UseCase)
                {
                    document.NewPage();
                    document.Add(tableUseCases(projectID));
                }

                if (Scenario)
                {
                    List<Scenario> scenarios = db.Scenarios.Where(s => s.ID_Project == projectID && s.Done && s.ID_MainScenario == null).ToList();

                    for (int i = 0; i < scenarios.Count; i++)
                    {
                        Scenario scenario = scenarios[i];
                        List<Scenario> alternativeScenarios = db.Scenarios.Where(a => a.ID_MainScenario == scenario.ID && a.Done).ToList();
                        document.NewPage();
                        if (i == 0)
                        {
                            document.Add(tableScenarios(scenario, true, false));
                        }
                        else
                        {
                            document.Add(tableScenarios(scenario, false, false));
                        }
                        foreach (var alt in alternativeScenarios)
                        {
                            document.NewPage();
                            document.Add(tableScenarios(alt, false, true));
                        }
                    }

                }
                byte[] byteInfo = workStream.ToArray();
                document.Close();
                //Response.ClearContent();
                //Response.Clear();
                //Response.ContentType = "application/pdf";
                //Response.AddHeader("content-disposition", "attachment;filename=" + fileName+";");

                //Response.OutputStream.Write(byteInfo, 0, byteInfo.Length);
                //Response.Flush();
                //Response.End();


                workStream.Write(byteInfo, 0, byteInfo.Length);
                workStream.Position = 0;

                return File(workStream, System.Net.Mime.MediaTypeNames.Application.Pdf, fileName);
            }catch(Exception ex)
            {
                string function = "console.log('{0}');";
                string log = string.Format(GenerateCodeFromFunction(function), ex.Message);
                Response.Write(log);
            }
            return null;
        }

        static string GenerateCodeFromFunction(string function)
        {
            string scriptTag = "<script type=\"\" language=\"\">{0}</script>";
            return string.Format(scriptTag, function);
        }

        private PdfPTable titlePage(int projectID)
        {
            PdfPTable tableLayout = new PdfPTable(1);
            float[] headers = { 100 };
            tableLayout.SetWidths(headers);
            tableLayout.WidthPercentage = 100;
            tableLayout.HeaderRows = 1;

            Project project = db.Projects.Find(projectID);

            string ARIALUNI_TFF = Server.MapPath("~/fonts/ARIALUNI.TTF");
            BaseFont bf = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            tableLayout.AddCell(new PdfPCell(new Phrase(project.Name, new Font(bf, 25, Font.BOLD, BaseColor.BLACK)))
            {
                Colspan = 12,
                Border = 0,
                PaddingTop = 150,
                PaddingBottom = 20,
                HorizontalAlignment = Element.ALIGN_CENTER
            });
            tableLayout.AddCell(new PdfPCell(new Phrase(project.Description, new Font(bf, 11, Font.NORMAL, BaseColor.BLACK)))
            {
                Colspan = 12,
                Border = 0,
                PaddingTop = 20,
                PaddingBottom = 20,
                HorizontalAlignment = Element.ALIGN_CENTER
            });
            tableLayout.AddCell(new PdfPCell(new Phrase("Autor projektu: " + project.User.Name + " " + project.User.Surname, new Font(bf, 11, Font.NORMAL, BaseColor.BLACK)))
            {
                Colspan = 12,
                Border = 0,
                PaddingTop = 300,
                HorizontalAlignment = Element.ALIGN_RIGHT,
            });

            tableLayout.AddCell(new PdfPCell(new Phrase("Datum založení projektu: " + project.DateCreated.Value.ToShortDateString(), new Font(bf, 11, Font.NORMAL, BaseColor.BLACK)))
            {
                Colspan = 12,
                Border = 0,
                Padding = 5,
                HorizontalAlignment = Element.ALIGN_RIGHT,
            });

            return tableLayout;
        }

        private PdfPTable tableTasks(int projectID)
        {
            PdfPTable tableLayout = new PdfPTable(7);
            float[] headers = { 50, 50, 100, 50, 50, 60, 50 };
            tableLayout.SetWidths(headers);
            tableLayout.WidthPercentage = 100;
            tableLayout.HeaderRows = 1;

            List<Task> tasks = db.Tasks.Where(t => t.ID_Project == projectID).ToList();
            string ARIALUNI_TFF = Server.MapPath("~/fonts/ARIALUNI.TTF");
            BaseFont bf = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            tableLayout.AddCell(new PdfPCell(new Phrase("Seznam úkolů", new Font(bf, 20, Font.BOLD, BaseColor.BLACK)))
            {
                Colspan = 12,
                Border = 0,
                PaddingBottom = 20,
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            AddCellToHeader(tableLayout, "Zadavatel");
            AddCellToHeader(tableLayout, "Vyřizovatel");
            AddCellToHeader(tableLayout, "Úkol");
            AddCellToHeader(tableLayout, "Deadline");
            AddCellToHeader(tableLayout, "Priorita");
            AddCellToHeader(tableLayout, "Stav");
            AddCellToHeader(tableLayout, "Dokončeno");

            foreach(var task in tasks)
            {
                AddCellToBody(tableLayout, task.User.Name+" "+task.User.Surname);
                AddCellToBody(tableLayout, task.User1.Name + " " + task.User1.Surname);
                AddCellToBody(tableLayout, task.Text);
                if (task.Deadline.HasValue)
                {
                    AddCellToBody(tableLayout, task.Deadline.Value.ToShortDateString());
                }
                else
                {
                    AddCellToBody(tableLayout, "");
                }
                
                AddCellToBody(tableLayout, task.PriorityTask.Priority);
                AddCellToBody(tableLayout, task.StateTask.State);
                if (task.DateFinished.HasValue)
                {
                    AddCellToBody(tableLayout, task.DateFinished.Value.ToShortDateString());
                }
                else
                {
                    AddCellToBody(tableLayout, "");
                }

            }

            return tableLayout;
        }

        private PdfPTable tableTeam(int projectID)
        {
            PdfPTable tableLayout = new PdfPTable(5);
            float[] headers = { 50,50,50,50,50};
            tableLayout.SetWidths(headers);
            tableLayout.WidthPercentage = 100;
            tableLayout.HeaderRows = 1;

            List<User> team = db.ProjectUsers.Where(p => p.ID_Project == projectID).Select(s => s.User).ToList();
            string ARIALUNI_TFF = Server.MapPath("~/fonts/ARIALUNI.TTF");
            BaseFont bf = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            tableLayout.AddCell(new PdfPCell(new Phrase("Členové týmu", new Font(bf, 20, Font.BOLD, BaseColor.BLACK)))
            {
                Colspan = 12,
                Border = 0,
                PaddingBottom = 20,
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            AddCellToHeader(tableLayout, "Jméno");
            AddCellToHeader(tableLayout, "Příjmení");
            AddCellToHeader(tableLayout, "Datum narození");
            AddCellToHeader(tableLayout, "Bydliště");
            AddCellToHeader(tableLayout, "Datum registrace");

            foreach(var t in team)
            {
                AddCellToBody(tableLayout, t.Name);
                AddCellToBody(tableLayout, t.Surname);
                if (t.BirthDate.HasValue)
                {
                    AddCellToBody(tableLayout, t.BirthDate.Value.ToShortDateString());
                }
                else
                {
                    AddCellToBody(tableLayout, "");
                }
                
                AddCellToBody(tableLayout, t.City);
                AddCellToBody(tableLayout, t.RegistrationDate.ToShortDateString());
            }

            return tableLayout;
        }
        private PdfPTable tableRequirements(int projectID)
        {
            PdfPTable tableLayout = new PdfPTable(7);
            float[] headers = { 50, 100, 50, 50, 50, 50, 50 };
            tableLayout.SetWidths(headers);
            tableLayout.WidthPercentage = 100;
            tableLayout.HeaderRows = 1;

            List<Requirement> requirements = db.Requirements.Where(r => r.ID_Project == projectID).ToList();

            string ARIALUNI_TFF = Server.MapPath("~/fonts/ARIALUNI.TTF");
            BaseFont bf = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            tableLayout.AddCell(new PdfPCell(new Phrase("Seznam požadavků", new Font(bf,20,Font.BOLD,BaseColor.BLACK)))
            {
                Colspan = 12,
                Border = 0,
                PaddingBottom = 20,
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            AddCellToHeader(tableLayout, "ID požadavku");
            AddCellToHeader(tableLayout, "Požadavek");
            AddCellToHeader(tableLayout, "Typ");
            AddCellToHeader(tableLayout, "Kategorie");
            AddCellToHeader(tableLayout, "Priorita");
            AddCellToHeader(tableLayout, "Zdroj");
            AddCellToHeader(tableLayout, "Status");

            foreach (var req in requirements)
            {
                AddCellToBody(tableLayout, req.ID_Requirement);
                AddCellToBody(tableLayout, req.Text);
                AddCellToBody(tableLayout, req.ReqType.Type);
                AddCellToBody(tableLayout, req.CategoryRequirement.Name);
                AddCellToBody(tableLayout, req.PriorityRequirement.Priority);
                AddCellToBody(tableLayout, req.Source);
                AddCellToBody(tableLayout, req.StatusRequirement.Status);
            }
            return tableLayout;
        }
      
        private PdfPTable tableScenarios(Scenario scenario,bool first, bool alternate)
        {
            PdfPTable tableLayout = new PdfPTable(1);
            float[] headers = { 100 };
            tableLayout.SetWidths(headers);
            tableLayout.WidthPercentage = 100;
            tableLayout.HeaderRows = 1;

            string ARIALUNI_TFF = Server.MapPath("~/fonts/ARIALUNI.TTF");
            BaseFont bf = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            if (first)
            {
                tableLayout.AddCell(new PdfPCell(new Phrase("Scénáře", new Font(bf, 20, Font.BOLD, BaseColor.BLACK)))
                {
                    Colspan = 12,
                    Border = 0,
                    PaddingBottom = 20,
                    HorizontalAlignment = Element.ALIGN_CENTER
                });
            }

            if (alternate)
            {
                tableLayout.AddCell(new PdfPCell(new Phrase("Alternativní scénář: " + scenario.UseCase.Name, new Font(bf, 8, Font.NORMAL, BaseColor.BLACK)))
                {
                    Padding = 5,
                    HorizontalAlignment = Element.ALIGN_CENTER
                });
            }
            else
            {
                tableLayout.AddCell(new PdfPCell(new Phrase("Případ užití: " + scenario.UseCase.Name, new Font(bf, 8, Font.NORMAL, BaseColor.BLACK)))
                {
                    Padding = 5,
                    HorizontalAlignment = Element.ALIGN_CENTER
                });
            }

     
            AddCellToBody(tableLayout, "ID: " + scenario.ID_Scenario);
            AddCellToBody(tableLayout, "Stručný popis:\n" + scenario.Description);
            List<Actor> mainActors = db.ScenarioActors.Where(s => s.ID_Scenario == scenario.ID && s.ID_ActorType == 1).Select(s=>s.Actor).ToList();
            List<Actor> otherActors = db.ScenarioActors.Where(s => s.ID_Scenario == scenario.ID && s.ID_ActorType == 2).Select(s => s.Actor).ToList();
            String mActors = "Hlavní aktéři:\n";

            for(int i = 0; i < mainActors.Count; i++)
            {
                mActors += mainActors[i].Name;
                if (i != (mainActors.Count - 1))
                {
                    mActors += ", ";
                }
            }
            if (mainActors.Count == 0)
            {
                AddCellToBody(tableLayout, "Hlavní aktéři:\nŽádní.");
            }
            else
            {
                AddCellToBody(tableLayout, mActors);
            }
            

            string oActors = "Vedlejší aktéři:\n";

            for (int i = 0; i < otherActors.Count; i++)
            {
                oActors += otherActors[i].Name;
                if (i != (otherActors.Count - 1))
                {
                    oActors += ", ";
                }
            }
            if (otherActors.Count == 0)
            {
                AddCellToBody(tableLayout, "Vedlejší aktéři:\nŽádní.");
            }
            else
            {
                AddCellToBody(tableLayout, oActors);
            }
            AddCellToBody(tableLayout, "Vstupní podmínky:\n" + scenario.InCondition);
            if (alternate)
            {
                AddCellToBody(tableLayout, "Alternativní scénář:" + Regex.Replace(scenario.Scenario1, "<.*?>", string.Empty));
            }
            else
            {
                AddCellToBody(tableLayout, "Hlavní scénář:" + Regex.Replace(scenario.Scenario1, "<.*?>", string.Empty));
            }
            AddCellToBody(tableLayout, "Výstupní podmínky:\n" + scenario.OutCondition);

            if (!alternate)
            {
                List<Scenario> alternateScenarios = db.Scenarios.Where(s => s.ID_MainScenario == scenario.ID && s.Done).ToList();
                string alScenarios = "Alternativní scénáře:\n";

                for (int i = 0; i < alternateScenarios.Count; i++)
                {
                    alScenarios += alternateScenarios[i].ID_Scenario;
                    if (i != (alternateScenarios.Count - 1))
                    {
                        alScenarios += ", ";
                    }
                }
                if (alternateScenarios.Count == 0)
                {
                    AddCellToBody(tableLayout, "Alternativní scénáře:\nŽádné.");
                }
                else
                {
                    AddCellToBody(tableLayout, alScenarios);
                }
            }



            return tableLayout;
        }
        private PdfPTable tableUseCases(int projectID)
        {
            PdfPTable tableLayout = new PdfPTable(3);
            float[] headers = { 50, 50, 50};
            tableLayout.SetWidths(headers);
            tableLayout.WidthPercentage = 100;
            tableLayout.HeaderRows = 1;

            List<UseCase> useCases = db.UseCases.Where(u => u.ID_Project == projectID).ToList();

            string ARIALUNI_TFF = Server.MapPath("~/fonts/ARIALUNI.TTF");
            BaseFont bf = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            tableLayout.AddCell(new PdfPCell(new Phrase("Případy užití", new Font(bf, 20, Font.BOLD, BaseColor.BLACK)))
            {
                Colspan = 12,
                Border = 0,
                PaddingBottom = 20,
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            AddCellToHeader(tableLayout, "Název případu užití");
            AddCellToHeader(tableLayout, "Popis");
            AddCellToHeader(tableLayout, "Realizované požadavky");

            foreach(var usecase in useCases)
            {
                AddCellToBody(tableLayout, usecase.Name);
                AddCellToBody(tableLayout, usecase.Description);
                var reqs = db.UseCaseRequirements.Where(r => r.ID_UseCase == usecase.ID).Select(s => s.Requirement).ToList();
                string names = "";
                for(int i = 0; i < reqs.Count; i++)
                {
                    names += reqs[i].ID_Requirement;
                    if (i != (reqs.Count - 1))
                    {
                        names += "\n";
                    }
                }
                AddCellToBody(tableLayout, names);
            }

            return tableLayout;
        }

        private PdfPTable tableActors(int projectID)
        {
            PdfPTable tableLayout = new PdfPTable(2);
            float[] headers = { 50,50};
            tableLayout.SetWidths(headers);
            tableLayout.WidthPercentage = 100;
            tableLayout.HeaderRows = 1;

            List<Actor> actors = db.Actors.Where(a => a.ID_Project == projectID).ToList();

            string ARIALUNI_TFF = Server.MapPath("~/fonts/ARIALUNI.TTF");
            BaseFont bf = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            tableLayout.AddCell(new PdfPCell(new Phrase("Seznam aktérů", new Font(bf, 20, Font.BOLD, BaseColor.BLACK)))
            {
                Colspan = 12,
                Border = 0,
                PaddingBottom = 20,
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            AddCellToHeader(tableLayout, "Název aktéra");
            AddCellToHeader(tableLayout, "Případy užití");
    

            foreach (var actor in actors)
            {
                AddCellToBody(tableLayout, actor.Name);
                var usecases = db.UseCaseActors.Where(x => x.ID_Actor == actor.ID).Select(s => s.UseCase).ToList();
                string names = "";
               for(int i = 0; i < usecases.Count; i++)
                {
                    names += usecases[i].Name;
                    if (i != (usecases.Count - 1))
                    {
                        names += "\n";
                    }
                }
                AddCellToBody(tableLayout, names);
            }
            return tableLayout;
        }

        private static void AddCellToHeader(PdfPTable tableLayout, string cellText)
        {
            string ARIALUNI_TFF = System.Web.HttpContext.Current.Server.MapPath("~/fonts/ARIALUNI.TTF");
            BaseFont bf = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(bf,9,Font.BOLD,BaseColor.WHITE)))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                Padding = 5,
                BackgroundColor = new iTextSharp.text.BaseColor(46, 109, 164)
            });
        }


        // Method to add single cell to the body  
        private static void AddCellToBody(PdfPTable tableLayout, string cellText)
        {
            string ARIALUNI_TFF = System.Web.HttpContext.Current.Server.MapPath("~/fonts/ARIALUNI.TTF");

            BaseFont bf = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(bf,8,Font.NORMAL,BaseColor.BLACK)))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                Padding = 5,
                BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
            });
        }

    }


    public class PageEventHelper : PdfPageEventHelper
    {
        PdfContentByte cb;
        PdfTemplate template;


        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            cb = writer.DirectContent;
            template = cb.CreateTemplate(50, 50);
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);
            string ARIALUNI_TFF = HttpContext.Current.Server.MapPath("~/fonts/ARIALUNI.TTF");
            BaseFont bf = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            int pageN = writer.PageNumber;
            string text = pageN.ToString() + "/";
            float len = bf.GetWidthPoint(text, 9f);

            iTextSharp.text.Rectangle pageSize = document.PageSize;

            cb.SetRGBColorFill(100, 100, 100);

            cb.BeginText();
            cb.SetFontAndSize(bf, 9);
            cb.SetTextMatrix(document.LeftMargin, pageSize.GetBottom(document.BottomMargin));
            cb.ShowText(text);

            cb.EndText();

            cb.AddTemplate(template, document.LeftMargin + len, pageSize.GetBottom(document.BottomMargin));
        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);
            string ARIALUNI_TFF = HttpContext.Current.Server.MapPath("~/fonts/ARIALUNI.TTF");
            BaseFont bf = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            template.BeginText();
            template.SetFontAndSize(bf, 9);
            template.SetTextMatrix(0, 0);
            template.ShowText("" + (writer.PageNumber));
            template.EndText();
        }


    }
}
