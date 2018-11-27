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

        [HttpPost]
        public ActionResult Index(bool Team, bool Requirement, bool UseCase, bool Scenario, int projectID)
        {
            int userID = (int)Session["userID"];
            var user = db.Users.Find(userID);
            var project = db.Projects.Find(projectID);

            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            string fileName = "Dokumentace " + DateTime.Now.ToShortDateString() + ".pdf";
            Document document = new Document();
            document.SetMargins(0, 0, 0, 0);


            string strAttachment = Server.MapPath("~/Downloads/" + fileName);
            PdfWriter.GetInstance(document, workStream).CloseStream = false;
            document.Open();

            document.AddAuthor(user.Name + " " + user.Surname);
            document.AddCreationDate();
            document.AddTitle("Projekt " + project.Name + " - dokumentace");

            if (Team)
            {
                document.Add(tableTeam(projectID));
            }

            if (Requirement)
            {
                document.Add(tableRequirements(projectID));
            }

            if (UseCase)
            {
                document.Add(tableUseCases(projectID));
            }

            if (Scenario)
            {
                document.Add(tableScenarios(projectID));
            }
            document.Close();

            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;

            return File(workStream, "application/pdf", fileName);
        }

        private PdfPTable tableRequirements(int projectID)
        {
            PdfPTable tableLayout = new PdfPTable(7);
            float[] headers = { 50, 50, 50, 50, 50, 50, 50 };
            tableLayout.SetWidths(headers);
            tableLayout.WidthPercentage = 100;
            tableLayout.HeaderRows = 1;

            List<RequirementView> requirements = db.RequirementViews.Where(r => r.ID_Project == projectID).AsNoTracking().ToList();
            tableLayout.AddCell(new PdfPCell(new Phrase("Seznam požadavků", new Font(Font.FontFamily.HELVETICA, 8, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 12,
                Border = 0,
                PaddingBottom = 5,
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
        private PdfPTable tableTeam(int projectID)
        {
            PdfPTable tableLayout = new PdfPTable(5);
            return tableLayout;
        }
        private PdfPTable tableScenarios(int projectID)
        {
            PdfPTable tableLayout = new PdfPTable(5);
            return tableLayout;
        }
        private PdfPTable tableUseCases(int projectID)
        {
            PdfPTable tableLayout = new PdfPTable(5);
            return tableLayout;
        }

        private static void AddCellToHeader(PdfPTable tableLayout, string cellText)
        {

            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.WHITE)))
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

            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(bf,12,Font.NORMAL,BaseColor.BLACK)))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                Padding = 5,
                BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
            });
        }

    }
}