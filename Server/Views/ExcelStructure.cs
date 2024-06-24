

using System.Numerics;
using MiniExcelLibs.Attributes;
using Server.Models;
namespace Server.Views;

public class ExcelOrgNrOnly
{
    public int Orgnummer { get; set; }
}
public class OrgNrTemplate
{
    [ExcelColumn(Name = "Orgnummer", Index = 0, Width = 20)]
    public int Orgnummer { get; set; }
    public static List<OrgNrTemplate> GenTemplate()
    {
        List<OrgNrTemplate> templateList = [];
        int orgNrTemplate = 12345673;
        for (int i = 0; i < 5; i++)
        {
            OrgNrTemplate template = new()
            {
                Orgnummer = orgNrTemplate + i
            };
            templateList.Add(template);
        }
        return templateList;
    }
}
public class UpdateDbTemplate
{
    [ExcelColumn(Name = "Rapportår", Index = 0, Width = 15)]
    public int RapportÅr { get; set; }
    [ExcelColumn(Name = "Orgnummer", Index = 1, Width = 20)]
    public int Orgnummer { get; set; }
    [ExcelColumn(Name = "Fase", Index = 2, Width = 25)]
    public string? Fase { get; set; }
    [ExcelColumn(Name = "Bransje", Index = 3, Width = 25)]
    public string? Bransje { get; set; }
    [ExcelColumn(Name = "KvinneligGrunder", Index = 4, Width = 25)]
    public int KvinneligGrunder { get; set; }
}
public class AnnouncementTable
{
    [ExcelColumn(Name = "Orgnummer", Index = 0, Width = 15)]
    public int Orgnumber { get; set; }
    [ExcelColumn(Name = "Målbedrift", Index = 1, Width = 25)]
    public string? Name { get; set; }
    [ExcelColumn(Name = "Id", Index = 2, Width = 15)]
    public BigInteger Id { get; set; }
    [ExcelColumn(Name = "Dato", Index = 3, Width = 20)]
    public DateOnly? Date { get; set; }
    [ExcelColumn(Name = "Beskrivelse", Index = 4, Width = 20)]
    public string? Description { get; set; }
    [ExcelColumn(Name = "type", Index = 5, Width = 25)]
    public string? Type { get; set; }
}