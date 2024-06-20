namespace Server.Models;
using MiniExcelLibs.Attributes;

public class FullView
{
    [ExcelColumn(Name = "Orgnummer", Index = 0, Width = 20)]
    public int Orgnumber { get; set; }
    [ExcelColumn(Name = "Målbedrift", Index = 1, Width = 20)]
    public string? CompanyName { get; set; }
    [ExcelColumn(Name = "Bransje", Index = 2, Width = 15)]
    public string? Branch { get; set; }
    [ExcelColumn(Name = "Fase", Index = 3, Width = 15)]
    public string? Phase { get; set; }
    [ExcelColumn(Name = "Likvidert", Index = 4, Width = 15)]
    public bool Liquidated { get; set; }
    [ExcelColumn(Name = "År", Index = 5, Width = 15)]
    public int Year { get; set; }
    [ExcelColumn(Name = "Øko Kode", Index = 6, Width = 15)]
    public string? EcoCode { get; set; }
    [ExcelColumn(Name = "Kode beskrivelse", Index = 7, Width = 20)]
    public string? CodeDescription { get; set; }
    [ExcelColumn(Name = "Verdi", Index = 8, Width = 15)]
    public decimal EcoValue { get; set; }
    [ExcelColumn(Name = "Delta", Index = 9, Width = 15)]
    public decimal Delta { get; set; }
    [ExcelColumn(Name = "Akkumulert", Index = 10, Width = 15)]
    public decimal Accumulated { get; set; }
    [ExcelColumn(Name = "Antall Ansatte", Index = 11, Width = 15)]
    public int NumberOfEmployees { get; set; }
    [ExcelColumn(Name = "Landsdel", Index = 12, Width = 20)]
    public string? CountryPart { get; set; }
    [ExcelColumn(Name = "Fylke", Index = 13, Width = 20)]
    public string? County { get; set; }
    [ExcelColumn(Name = "Kommune", Index = 14, Width = 20)]
    public string? Municipality { get; set; }
    [ExcelColumn(Name = "Postadresse", Index = 15, Width = 20)]
    public string? AdressLine { get; set; }
    [ExcelColumn(Name = "Postkode", Index = 16, Width = 15)]
    public string? ZipCode { get; set; }
    [ExcelColumn(Name = "Tittel", Index = 17, Width = 15)]
    public string? Title { get; set; }
    [ExcelColumn(Name = "Navn", Index = 18, Width = 20)]
    public string? Name { get; set; }
    [ExcelColumn(Name = "Tittelkode", Index = 19, Width = 15)]
    public string? TitleCode { get; set; }
}