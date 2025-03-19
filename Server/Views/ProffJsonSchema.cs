
using System.Text.Json;

namespace Server.Views;
using Microsoft.EntityFrameworkCore;
using Server.Util;
using Server.Models;
using Server.Context;



public abstract class EcoCodes
{
    public required string Code { get; set; }
    public required string Amount { get; set; }
}
public abstract class AccountsInfo
{
    public string? AccIncompleteCode { get; set; }
    public string? AccIncompleteDesc { get; set; }
    public required string Year { get; set; }
    public required List<EcoCodes> Accounts { get; set; }

}
public abstract class WebInfo
{
    public string? Href { get; set; }
    public string? Rel { get; set; }
}
public abstract class ShareHolderInfo
{
    public string? CompanyId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Name { get; set; }
    public decimal NumberOfShares { get; set; }
    public required string Share { get; set; }
    public WebInfo? Details { get; set; }
}

public abstract class Announcement
{
    public required string Id { get; set; }
    public required string Date { get; set; }
    public required string Text { get; set; }
    public required string Type { get; set; }
}
public abstract class LocationInfo
{
    public required string CountryPart { get; set; }
    public required string County { get; set; }
    public required string Municipality { get; set; }
}
public abstract class PostalInfo
{
    public required string AddressLine { get; set; }
    public required string ZipCode { get; set; }
}
public abstract class PersonRole
{
    public required string BirthDate { get; set; }
    public required string Name { get; set; }
    public required string Title { get; set; }
    public required string TitleCode { get; set; }
}
/* Må lage en class basert på hva data som skal hentes inn fra proff. basert på JSON schema. Se på proffjson schema for referanse. */
public class ReturnStructure
{
    public string? CompanyType { get; set; }
    public string? CompanyTypeName { get; set; }
    public List<string>? PreviousNames { get; set; }
    public string? RegistrationDate { get; set; }
    public string? EstablishedDate { get; set; }
    public required string ShareholdersLastUpdatedDate { get; set; }
    public string? NumberOfEmployees { get; set; }
    public required string CompanyId { get; set; }
    public required string Name { get; set; }
    /* Rollekoder vi er ute etter fra PROFF er DAGL, eller LEDE hvis DAGL ikke er tilgjengelig. */
    public required List<PersonRole> PersonRoles { get; set; }
    public string? LiquidationDate { get; set; }
    public required List<Announcement> Announcements { get; set; }
    public required List<AccountsInfo> CompanyAccounts { get; set; }
    public List<ShareHolderInfo>? Shareholders { get; set; }
    public required LocationInfo Location { get; set; }
    public required PostalInfo PostalAddress { get; set; }

    public RawDataFromProff ConvertToRawData()
    {
        var jsonFormat = JsonSerializer.Serialize(this);
        var RawData = new RawDataFromProff()
        {
            RawData = jsonFormat,
        };
        return RawData;
    }
    public async Task InsertIntoDatabase(BtdbContext context)
    {
        
        var bedriftId = context.CompanyInfos.Single(b => b.Orgnumber == int.Parse(CompanyId)).CompanyId;
        var result = false || LiquidationDate != null && !string.IsNullOrEmpty(LiquidationDate);
        new UpdateNameStructure(
                    Name, result, PreviousNames?.Count == 0 ? null : PreviousNames
                ).CraftDbValues(context, bedriftId);
        var genInfo = new InsertGeneralInfoStructure(
            ShareholdersLastUpdatedDate, Location, PostalAddress, NumberOfEmployees ?? null
        ).CraftDbValues(bedriftId);
        try
        {
            await UpsertHandler.UpsertEntity(context, genInfo);
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine($"DbUpdateException occured: {ex.Message}");
        }
        List<CompanyAnnouncement> annList = [];
        annList.AddRange(Announcements.Select(announcement => new InsertAnnouncementStructure(announcement).CraftDbValues(bedriftId)));
        try
        {
            foreach (var entity in annList)
            {
                await UpsertHandler.UpsertEntity(context, entity);
            }
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine($"DbUpdateException occured: {ex.Message}");
        }
        foreach (var ecoList in CompanyAccounts.Select(account => new EcoDataStructure(
                     account
                 ).CraftDbValues(bedriftId)))
        {
            try
            {
                foreach (var entity in ecoList)
                {
                    await UpsertHandler.UpsertEntity(context, entity);
                };
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException occured: {ex.Message}");
            }
        }

        Dictionary<string, PersonRole> valuePairs = [];
        foreach (var person in PersonRoles)
        {
            if (person.TitleCode != "DAGL" && person.TitleCode != "LEDE") continue;
            else
            {
                valuePairs[person.TitleCode] = person;
            }
        }
        foreach (var pair in valuePairs)
        {
            try
            {
                await UpsertHandler.UpsertEntity(context, new CompanyLeaderStructure(
                ShareholdersLastUpdatedDate, pair.Value
            ).CraftDbValues(bedriftId));
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Update error occured: {ex.Message}");
            }
        }
        if (Shareholders == null) return;
        {
            foreach (var shareholder in Shareholders)
            {
                try
                {
                    await UpsertHandler.UpsertEntity(context, new InsertShareholderStructure(
                        ShareholdersLastUpdatedDate, shareholder
                    ).CraftDbValues(bedriftId));
                }
                catch (DbUpdateException ex)
                {
                    Console.WriteLine($"Update failed: {ex.Message}");
                }
            }
        }
    }
}

public class EcoDataStructure
{
    public int InputYear { get; set; }
    public Dictionary<string, decimal> CodeValuePairs { get; set; }
    public EcoDataStructure(AccountsInfo accounts)
    {
        var parsingNr = int.TryParse(accounts.Year, result: out var tempOutput);
        if (!parsingNr) throw new ArgumentException($"Could not parse {accounts.Year} to an integer.");
        InputYear = tempOutput;
        CodeValuePairs = [];
        foreach (var account in accounts.Accounts)
            CodeValuePairs[account.Code] = decimal.TryParse(account.Amount, out var result) ? Math.Round(result, 4) : 0;
    }
    public List<CompanyEconomicDataPrYear> CraftDbValues(int bedriftId)
    {
        try
        {
            List<CompanyEconomicDataPrYear> dataList = [];
            dataList.AddRange(CodeValuePairs.Select(pair => new CompanyEconomicDataPrYear { CompanyId = bedriftId, EcoCode = pair.Key, EcoValue = pair.Value, Year = InputYear }));
            return dataList;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to craft object, {ex.Message}");
        }
    }
}
public class UpdateNameStructure
{
    public string Name { get; set; }
    public bool Liquidated { get; set; }
    public List<string>? PreviousNames { get; set; }
    public UpdateNameStructure(string name, bool isLiquidated, List<string>? previousNames = null)
    {
        Name = name;
        if (previousNames != null) PreviousNames = previousNames;
        Liquidated = isLiquidated;
    }
    public void CraftDbValues(BtdbContext context, int companyId)
    {
        try
        {

            var companyInfo = context.CompanyInfos.Single(b => b.CompanyId == companyId);
            companyInfo.CompanyName = Name;
            companyInfo.Liquidated = Liquidated;
            if (PreviousNames != null && PreviousNames.Count > 0) companyInfo.PrevNames = PreviousNames;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
public class InsertShareholderStructure
{
    public int InputYear { get; set; }
    public decimal InputNumberOfShares { get; set; }
    public string InputName { get; set; }
    public string InputPercentage { get; set; }
    public string? ShareholderCId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public InsertShareholderStructure(string updatedYear, ShareHolderInfo info)
    {
        var parseSuccess = int.TryParse(updatedYear, out int output);
        if (!parseSuccess) throw new ArgumentException($"Couldn't parse {updatedYear} to integer");
        InputYear = output;

        InputNumberOfShares = info.NumberOfShares;
        InputName = info.Name ?? "Ingen navn funnet";
        InputPercentage = info.Share;
        if (!string.IsNullOrEmpty(info.CompanyId)) ShareholderCId = info.CompanyId;
        if (!string.IsNullOrEmpty(info.FirstName)) FirstName = info.FirstName;
        if (!string.IsNullOrEmpty(info.LastName)) LastName = info.LastName;
    }
    public CompanyShareholderInfo CraftDbValues(int bedriftId)
    {
        try
        {
            var shareholder = new CompanyShareholderInfo
            {
                CompanyId = bedriftId,
                Year = InputYear,
                ShareholdeCompanyId = ShareholderCId,
                ShareholderFirstName = FirstName,
                ShareholderLastName = LastName,
                PercentageShares = InputPercentage,
                Name = InputName,
                NumberOfShares = InputNumberOfShares
            };
            return shareholder;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to craft object, {ex.Message}");
        }
    }
}
public class InsertAnnouncementStructure
{
    public int OrgNr { get; set; }
    public long InputId { get; set; }
    public DateOnly? InputDate { get; set; }
    public string Inputdesc { get; set; }
    public string InputType { get; set; }
    public InsertAnnouncementStructure(Announcement announcement)
    {
        bool parseSuccess;
        parseSuccess = long.TryParse(announcement.Id, out long lOutput);
        if (!parseSuccess) throw new ArgumentException($"Couldn't parse {announcement.Id} to BigInt");
        InputId = lOutput;
        InputDate = string.IsNullOrEmpty(announcement.Date) ? null : DateCorrector.ConvertDate(announcement.Date);
        InputType = announcement.Type;
        Inputdesc = announcement.Text;
    }
    public CompanyAnnouncement CraftDbValues(int bedriftId)
    {
        try
        {

            var announcement = new CompanyAnnouncement
            {
                CompanyId = bedriftId,
                Date = InputDate,
                AnnouncementId = InputId,
                AnnouncementText = Inputdesc,
                AnnouncementType = InputType
            };
            return announcement;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to craft Object {ex.Message}");
        }
    }
}
public class InsertGeneralInfoStructure
{
    public int InputYear { get; set; }
    public string InputCountryPart { get; set; }
    public string InputCounty { get; set; }
    public string InputMunicipality { get; set; }
    public string InputZipCode { get; set; }
    public string InputAddressLine { get; set; }
    public int? InputNumberOfEmployees { get; set; }
    public InsertGeneralInfoStructure(string UpdateYear, LocationInfo location, PostalInfo post, string? NumberOfEmployees = null)
    {
        bool parseSuccess;
        parseSuccess = int.TryParse(UpdateYear, out int output);
        if (!parseSuccess) throw new ArgumentException($"Couldn't parse {UpdateYear} to Integer");
        InputYear = output;
        parseSuccess = int.TryParse(NumberOfEmployees, out output);
        if (!parseSuccess) InputNumberOfEmployees = 2;
        else InputNumberOfEmployees = output;
        InputCountryPart = location.CountryPart;
        InputCounty = location.County;
        InputMunicipality = location.Municipality;
        InputZipCode = string.IsNullOrEmpty(post.ZipCode) ? "Mangler PostKode" : post.ZipCode;
        InputAddressLine = post.AddressLine;
    }
    public GeneralYearlyUpdatedCompanyInfo CraftDbValues(int bedriftId)
    {
        try
        {
            var genInfo = new GeneralYearlyUpdatedCompanyInfo
            {
                CompanyId = bedriftId,
                CountryPart = InputCountryPart,
                County = InputCounty,
                Municipality = InputMunicipality,
                AdressLine = InputAddressLine,
                ZipCode = InputZipCode,
                Year = InputYear,
                NumberOfEmployees = InputNumberOfEmployees
            };
            return genInfo;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to craft object, {ex.Message}");
        }
    }
}
public class CompanyLeaderStructure
{
    public int InputYear { get; set; }
    public string InputName { get; set; }
    public string InputTitle { get; set; }
    public string InputTitleCode { get; set; }
    public DateOnly? InputDayOfBirth { get; set; }
    public CompanyLeaderStructure(string UpdateYear, PersonRole person)
    {
        bool parseSuccess;
        parseSuccess = int.TryParse(UpdateYear, out int output);
        if (!parseSuccess) throw new ArgumentException($"Couldn't parse {UpdateYear} into Integer");
        InputYear = output;
        InputName = person.Name;
        InputDayOfBirth = string.IsNullOrEmpty(person.BirthDate) ? null : DateCorrector.CorrectDate(person.BirthDate);
        InputTitle = person.Title;
        InputTitleCode = person.TitleCode;
    }
    public CompanyLeaderOverview CraftDbValues(int bedriftId)
    {
        try
        {
            var leader = new CompanyLeaderOverview
            {
                CompanyId = bedriftId,
                Name = InputName,
                DayOfBirth = InputDayOfBirth,
                Title = InputTitle,
                TitleCode = InputTitleCode,
                Year = InputYear
            };
            return leader;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to craft Object, {ex.Message}");
        }
    }
}