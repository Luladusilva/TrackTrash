using ClosedXML.Excel;
using TrackTrash.Application.Dtos;
using TrackTrash.Application.Ports.Services;

namespace TrackTrash.Infraestructure.Adapters.Services;

public class XMLService : IXMLService
{
    public byte[] CreateUsersReport(IReadOnlyList<UserDto> users)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Usuarios");

        worksheet.Cell(1, 1).Value = "ID";
        worksheet.Cell(1, 2).Value = "Nombre";
        worksheet.Cell(1, 3).Value = "Apellido";
        worksheet.Cell(1, 4).Value = "Correo";
        worksheet.Cell(1, 5).Value = "Rol";
        worksheet.Cell(1, 6).Value = "Activo";
        worksheet.Cell(1, 7).Value = "Fecha de registro";

        for (var i = 0; i < users.Count; i++)
        {
            var row = i + 2;
            var user = users[i];

            worksheet.Cell(row, 1).Value = user.UserId;
            worksheet.Cell(row, 2).Value = user.FirstName;
            worksheet.Cell(row, 3).Value = user.LastName;
            worksheet.Cell(row, 4).Value = user.Email;
            worksheet.Cell(row, 5).Value = user.RoleId.ToString();
            worksheet.Cell(row, 6).Value = user.IsActive ? "Sí" : "No";
            worksheet.Cell(row, 7).Value = user.CreatedAt;
        }

        FormatWorksheet(worksheet, 7);
        return SaveWorkbook(workbook);
    }

    public byte[] CreateCollectRequestsReport(IReadOnlyList<CollectRequestDto> requests)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Solicitudes");

        worksheet.Cell(1, 1).Value = "ID";
        worksheet.Cell(1, 2).Value = "Usuario";
        worksheet.Cell(1, 3).Value = "Organización";
        worksheet.Cell(1, 4).Value = "Ubicación";
        worksheet.Cell(1, 5).Value = "Fecha de solicitud";
        worksheet.Cell(1, 6).Value = "Fecha programada";
        worksheet.Cell(1, 7).Value = "Hora programada";
        worksheet.Cell(1, 8).Value = "Estado";
        worksheet.Cell(1, 9).Value = "Observaciones";

        for (var i = 0; i < requests.Count; i++)
        {
            var row = i + 2;
            var request = requests[i];

            worksheet.Cell(row, 1).Value = request.CollectRequestId;
            worksheet.Cell(row, 2).Value = request.UserId;
            worksheet.Cell(row, 3).Value = request.OrganizationId?.ToString() ?? string.Empty;
            worksheet.Cell(row, 4).Value = request.LocationId;
            worksheet.Cell(row, 5).Value = request.RequestedAt;
            worksheet.Cell(row, 6).Value = request.ScheduledDate.ToString("yyyy-MM-dd");
            worksheet.Cell(row, 7).Value = request.ScheduledTime.ToString("HH:mm");
            worksheet.Cell(row, 8).Value = request.StatusId.ToString();
            worksheet.Cell(row, 9).Value = request.Observations ?? string.Empty;
        }

        FormatWorksheet(worksheet, 9);
        return SaveWorkbook(workbook);
    }

    private static void FormatWorksheet(IXLWorksheet worksheet, int columns)
    {
        var header = worksheet.Range(1, 1, 1, columns);
        header.Style.Font.Bold = true;
        header.Style.Fill.BackgroundColor = XLColor.LightBlue;
        worksheet.SheetView.FreezeRows(1);
        worksheet.Columns().AdjustToContents();
    }

    private static byte[] SaveWorkbook(XLWorkbook workbook)
    {
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
