using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TerraLink.Api.Common;

public static class PdfGenerator
{
    public static byte[] GenerateRepaymentSchedulePdf(IEnumerable<TerraLink.Api.DTOs.RepaymentSchedule.RepaymentScheduleResponse> schedule)
    {
        var lines = new List<string>
        {
            "%PDF-1.4",
            "1 0 obj",
            "<< /Type /Catalog /Pages 2 0 R >>",
            "endobj",
            "2 0 obj",
            "<< /Type /Pages /Kids [3 0 R] /Count 1 >>",
            "endobj",
            "3 0 obj",
            "<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Contents 4 0 R /Resources << /Font << /F1 5 0 R >> >> >>",
            "endobj",
            "4 0 obj",
            "<< /Length ",
            "5 0 obj",
            "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>",
            "endobj"
        };

        var content = new StringBuilder();
        content.AppendLine("BT");
        content.AppendLine("/F1 12 Tf");
        content.AppendLine("100 700 Td");
        content.AppendLine("(Repayment Schedule) Tj");
        content.AppendLine("0 -20 Td");

        foreach (var item in schedule)
        {
            content.AppendLine($"({item.InstallmentNumber}. Due: {item.DueDate:yyyy-MM-dd} | Principal: {item.Principal:N2} | Interest: {item.Interest:N2} | Total: {item.TotalDue:N2} | {item.Status}) Tj");
            content.AppendLine("0 -15 Td");
        }

        content.AppendLine("ET");

        var contentBytes = Encoding.ASCII.GetBytes(content.ToString());
        var contentLength = contentBytes.Length;

        lines[7] = $"<< /Length {contentLength} >>";
        var streamIndex = lines.Count;
        lines.Add("stream");
        lines.Add(content.ToString());
        lines.Add("endstream");
        lines.Add("endobj");

        var xrefOffset = lines.Sum(l => l.Length + 1);
        lines.Add("xref");
        lines.Add($"0 {streamIndex + 1}");
        lines.Add("0000000000 65535 f ");
        var offset = 9;
        foreach (var line in lines.Take(streamIndex))
        {
            lines.Add($"{offset:D10} 00000 n ");
            offset += line.Length + 1;
        }
        lines.Add("trailer");
        lines.Add($"<< /Size {streamIndex + 1} /Root 1 0 R >>");
        lines.Add("startxref");
        lines.Add(xrefOffset.ToString());
        lines.Add("%%EOF");

        return Encoding.ASCII.GetBytes(string.Join("\n", lines));
    }

    public static byte[] GenerateCertificatePdf(TerraLink.Api.DTOs.LoanClosures.CloseLoanResponse closure)
    {
        var lines = new List<string>
        {
            "%PDF-1.4",
            "1 0 obj",
            "<< /Type /Catalog /Pages 2 0 R >>",
            "endobj",
            "2 0 obj",
            "<< /Type /Pages /Kids [3 0 R] /Count 1 >>",
            "endobj",
            "3 0 obj",
            "<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Contents 4 0 R /Resources << /Font << /F1 5 0 R >> >> >>",
            "endobj",
            "4 0 obj",
            "<< /Length ",
            "5 0 obj",
            "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>",
            "endobj"
        };

        var content = new StringBuilder();
        content.AppendLine("BT");
        content.AppendLine("/F1 12 Tf");
        content.AppendLine("100 700 Td");
        content.AppendLine("(Loan Completion Certificate) Tj");
        content.AppendLine("0 -30 Td");
        content.AppendLine($"(Certificate Number: {closure.CertificateNumber}) Tj");
        content.AppendLine("0 -20 Td");
        content.AppendLine($"(Loan ID: {closure.LoanId}) Tj");
        content.AppendLine("0 -20 Td");
        content.AppendLine($"(Closure Date: {closure.ClosureDate:yyyy-MM-dd}) Tj");
        content.AppendLine("0 -20 Td");
        content.AppendLine("(This certifies that the above loan has been fully repaid.) Tj");
        content.AppendLine("ET");

        var contentBytes = Encoding.ASCII.GetBytes(content.ToString());
        var contentLength = contentBytes.Length;

        lines[7] = $"<< /Length {contentLength} >>";
        var streamIndex = lines.Count;
        lines.Add("stream");
        lines.Add(content.ToString());
        lines.Add("endstream");
        lines.Add("endobj");

        var xrefOffset = lines.Sum(l => l.Length + 1);
        lines.Add("xref");
        lines.Add($"0 {streamIndex + 1}");
        lines.Add("0000000000 65535 f ");
        var offset = 9;
        foreach (var line in lines.Take(streamIndex))
        {
            lines.Add($"{offset:D10} 00000 n ");
            offset += line.Length + 1;
        }
        lines.Add("trailer");
        lines.Add($"<< /Size {streamIndex + 1} /Root 1 0 R >>");
        lines.Add("startxref");
        lines.Add(xrefOffset.ToString());
        lines.Add("%%EOF");

        return Encoding.ASCII.GetBytes(string.Join("\n", lines));
    }
}
