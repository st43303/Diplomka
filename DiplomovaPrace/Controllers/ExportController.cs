using DiplomovaPrace.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DiplomovaPrace.Controllers
{
    public class ExportController : Controller
    {
        private SDTEntities db = Database.GetDatabase();
        private Document document;
        private PdfWriter pdfWriter;


        [HttpPost]
        public ActionResult Index(bool Team, bool Requirement, bool UseCase, bool Scenario, bool Actors, int projectID)
        {
            int userID = (int)Session["userID"];
            var user = db.Users.Find(userID);
            var project = db.Projects.Find(projectID);

            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            string fileName = "Dokumentace " + DateTime.Now.ToShortDateString() + ".pdf";
            document = new Document();
            document.SetMargins(0, 0, 0, 0);


            string strAttachment = Server.MapPath("~/Downloads/" + fileName);
            pdfWriter = PdfWriter.GetInstance(document, workStream);
            pdfWriter.CloseStream = false;
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
                List<Scenario> scenarios = db.Scenarios.Where(s => s.ID_Project == projectID && s.Done && s.ID_MainScenario==null).ToList();
 
                for(int i = 0; i < scenarios.Count; i++)
                {
                    document.NewPage();
                    if (i == 0)
                    {
                        document.Add(tableScenarios(scenarios[i],true));
                    }
                    else
                    {
                        document.Add(tableScenarios(scenarios[i], false));
                    }
                }
                
            }
            document.Close();

            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;

            return File(workStream, "application/pdf", fileName);
        }

        private PdfPTable titlePage(int projectID)
        {
            PdfPTable tableLayout = new PdfPTable(1);
            float[] headers = { 100 };
            tableLayout.SetWidths(headers);
            tableLayout.WidthPercentage = 80;
            tableLayout.HeaderRows = 1;

            ProjectInfoView project = db.ProjectInfoViews.Find(projectID);

            string ARIALUNI_TFF = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "ARIALUNI.TTF");
            BaseFont bf = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            tableLayout.AddCell(new PdfPCell(new Phrase(project.ProjectName + " - dokumentace", new Font(bf, 25, Font.BOLD, BaseColor.BLACK)))
            {
                Colspan = 12,
                Border = 0,
                PaddingTop = 250,
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
            tableLayout.AddCell(new PdfPCell(new Phrase("Autor projektu: " + project.Name + " " + project.Surname, new Font(bf, 11, Font.NORMAL, BaseColor.BLACK)))
            {
                Colspan = 12,
                Border = 0,
                PaddingTop = 300,
                HorizontalAlignment = Element.ALIGN_RIGHT,
            });

            tableLayout.AddCell(new PdfPCell(new Phrase("Datum zahájení projektu: " + project.DateCreated.Value.ToShortDateString(), new Font(bf, 11, Font.NORMAL, BaseColor.BLACK)))
            {
                Colspan = 12,
                Border = 0,
                Padding = 5,
                HorizontalAlignment = Element.ALIGN_RIGHT,
            });

            return tableLayout;
        }

        private PdfPTable tableTeam(int projectID)
        {
            PdfPTable tableLayout = new PdfPTable(4);
            float[] headers = { 50,50,50,50};
            tableLayout.SetWidths(headers);
            tableLayout.WidthPercentage = 80;
            tableLayout.HeaderRows = 1;

            List<TeamView> team = db.TeamViews.Where(r => r.ID_Project == projectID).AsNoTracking().ToList();
            string ARIALUNI_TFF = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "ARIALUNI.TTF");
            BaseFont bf = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            tableLayout.AddCell(new PdfPCell(new Phrase("Členové týmu", new Font(bf, 20, Font.BOLD, BaseColor.BLACK)))
            {
                Colspan = 12,
                Border = 0,
                PaddingBottom = 20,
                PaddingTop = 20,
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            AddCellToHeader(tableLayout, "Jméno");
            AddCellToHeader(tableLayout, "Příjmení");
            AddCellToHeader(tableLayout, "Datum narození");
            AddCellToHeader(tableLayout, "Bydliště");

            foreach(var t in team)
            {
                AddCellToBody(tableLayout, t.Name);
                AddCellToBody(tableLayout, t.Surname);
                AddCellToBody(tableLayout, t.BirthDate.Value.ToShortDateString());
                AddCellToBody(tableLayout, t.City);
            }

            return tableLayout;
        }
        private PdfPTable tableRequirements(int projectID)
        {
            PdfPTable tableLayout = new PdfPTable(7);
            float[] headers = { 50, 100, 50, 50, 50, 50, 50 };
            tableLayout.SetWidths(headers);
            tableLayout.WidthPercentage = 80;
            tableLayout.HeaderRows = 1;

            List<RequirementView> requirements = db.RequirementViews.Where(r => r.ID_Project == projectID).AsNoTracking().ToList();

            string ARIALUNI_TFF = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "ARIALUNI.TTF");
            BaseFont bf = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            tableLayout.AddCell(new PdfPCell(new Phrase("Seznam požadavků", new Font(bf,20,Font.BOLD,BaseColor.BLACK)))
            {
                Colspan = 12,
                Border = 0,
                PaddingBottom = 20,
                PaddingTop = 20,
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
                AddCellToBody(tableLayout, req.Type);
                AddCellToBody(tableLayout, req.Category);
                AddCellToBody(tableLayout, req.Priority);
                AddCellToBody(tableLayout, req.Source);
                AddCellToBody(tableLayout, req.Status);
            }
            return tableLayout;
        }
      
        private PdfPTable tableScenarios(Scenario scenario,bool first)
        {
            PdfPTable tableLayout = new PdfPTable(1);
            float[] headers = { 100 };
            tableLayout.SetWidths(headers);
            tableLayout.WidthPercentage = 80;
            tableLayout.HeaderRows = 1;

            string ARIALUNI_TFF = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "ARIALUNI.TTF");
            BaseFont bf = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            if (first)
            {
                tableLayout.AddCell(new PdfPCell(new Phrase("Scénáře", new Font(bf, 20, Font.BOLD, BaseColor.BLACK)))
                {
                    Colspan = 12,
                    Border = 0,
                    PaddingBottom = 20,
                    PaddingTop = 20,
                    HorizontalAlignment = Element.ALIGN_CENTER
                });
            }

            tableLayout.AddCell(new PdfPCell(new Phrase("Případ užití: "+scenario.UseCase.Name, new Font(bf, 8, Font.NORMAL, BaseColor.BLACK)))
            {
                Padding=5,
                HorizontalAlignment = Element.ALIGN_CENTER
            });
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
            

            String oActors = "Vedlejší aktéři:\n";

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
            AddCellToBody(tableLayout, "Hlavní scénář:\n" + scenario.Scenario1);
            AddCellToBody(tableLayout, "Výstupní podmínky:\n" + scenario.OutCondition);

            List<Scenario> alternateScenarios = db.Scenarios.Where(s => s.ID_MainScenario == scenario.ID).ToList();
            String alScenarios = "Alternativní scénáře:\n";

            for(int i = 0;i< alternateScenarios.Count; i++)
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

            return tableLayout;
        }
        private PdfPTable tableUseCases(int projectID)
        {
            PdfPTable tableLayout = new PdfPTable(3);
            float[] headers = { 50, 50, 50};
            tableLayout.SetWidths(headers);
            tableLayout.WidthPercentage = 80;
            tableLayout.HeaderRows = 1;

            List<UseCase> useCases = db.UseCases.Where(u => u.ID_Project == projectID).ToList();

            string ARIALUNI_TFF = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "ARIALUNI.TTF");
            BaseFont bf = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            tableLayout.AddCell(new PdfPCell(new Phrase("Případy užití", new Font(bf, 20, Font.BOLD, BaseColor.BLACK)))
            {
                Colspan = 12,
                Border = 0,
                PaddingBottom = 20,
                PaddingTop = 20,
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
                String names = "";
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
            tableLayout.WidthPercentage = 80;
            tableLayout.HeaderRows = 1;

            List<Actor> actors = db.Actors.Where(a => a.ID_Project == projectID).ToList();

            string ARIALUNI_TFF = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "ARIALUNI.TTF");
            BaseFont bf = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            tableLayout.AddCell(new PdfPCell(new Phrase("Seznam aktérů", new Font(bf, 20, Font.BOLD, BaseColor.BLACK)))
            {
                Colspan = 12,
                Border = 0,
                PaddingBottom = 20,
                PaddingTop = 20,
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            AddCellToHeader(tableLayout, "Název aktéra");
            AddCellToHeader(tableLayout, "Případy užití");
    

            foreach (var actor in actors)
            {
                AddCellToBody(tableLayout, actor.Name);
                var usecases = db.UseCaseActors.Where(x => x.ID_Actor == actor.ID).Select(s => s.UseCase).ToList();
                String names = "";
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
            string ARIALUNI_TFF = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "ARIALUNI.TTF");
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
            string ARIALUNI_TFF = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "ARIALUNI.TTF");

            BaseFont bf = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(bf,8,Font.NORMAL,BaseColor.BLACK)))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                Padding = 5,
                BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
            });
        }

    }


    public class PDFFooter : PdfPageEventHelper
    {
        // write on top of document
        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            base.OnOpenDocument(writer, document);
            PdfPTable tabFot = new PdfPTable(new float[] { 1F });
            tabFot.SpacingAfter = 10F;
            PdfPCell cell;
            tabFot.TotalWidth = 300F;
            cell = new PdfPCell(new Phrase(""));
            cell.Border = Rectangle.NO_BORDER;
            tabFot.AddCell(cell);
            tabFot.WriteSelectedRows(0, -1, 150, document.Top, writer.DirectContent);
        }

        // write on start of each page
        public override void OnStartPage(PdfWriter writer, Document document)
        {
            base.OnStartPage(writer, document);
        }

        // write on end of each page
        public override void OnEndPage(PdfWriter writer, Document document)
        {
            DateTime horario = DateTime.Now;
            base.OnEndPage(writer, document);
            PdfPTable tabFot = new PdfPTable(new float[] { 1F });
            PdfPCell cell;
            tabFot.TotalWidth = 300F;
            cell = new PdfPCell(new Phrase("TEST" + " - " + horario));
            cell.Border = Rectangle.NO_BORDER;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            tabFot.AddCell(cell);
            tabFot.WriteSelectedRows(0, -1, 150, document.Bottom, writer.DirectContent);
        }

        //write on close of document
        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);
        }
    }
}
