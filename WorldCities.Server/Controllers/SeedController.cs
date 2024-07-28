using Microsoft.AspNetCore.Mvc;
using WorldCities.Server.Data;
using System.Security;
using OfficeOpenXml;
using Microsoft.EntityFrameworkCore;
using WorldCities.Server.Data.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace WorldCities.Server.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class SeedController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _configuration;

    public SeedController(ApplicationDbContext context,
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager,
        IWebHostEnvironment env,
        IConfiguration configuration)
    {
        _context = context;
        _roleManager = roleManager;
        _userManager = userManager;
        _env = env;
        _configuration = configuration;
    }
    [HttpGet]
    public async Task<ActionResult> Import()
    {
        if (!_env.IsDevelopment())
        {
            throw new SecurityException("Not allowed");
        }

        var path = Path.Combine(
            _env.ContentRootPath,
            "Data/Source/worldcities.xlsx");

        using var stream = System.IO.File.OpenRead(path);
        using var excelPackage = new ExcelPackage(stream);

        // get the first worksheet
        var worksheet = excelPackage.Workbook.Worksheets[0];

        // define how many rows we want to process
        var nEndRow = worksheet.Dimension.End.Row;
        var nEndColumn = worksheet.Dimension.End.Column;
     

        // create a lookup dictionary
        // containing all the countries already existing
        // into the Database (it will be empty on first run)
        var countriesByName = _context.Countries
            .AsNoTracking()
            .ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);

        HashSet<CountrySet> countries = ExtractCountries(worksheet, nEndRow, nEndColumn);
        Console.WriteLine($"Country set count: {countries.Count}");

        int numberOfCountriesAdded = await InsertCountriesIntoDb(countriesByName, countries);
        
        HashSet<CitySet> citySets = ExtractCities(worksheet, nEndRow, nEndColumn, countriesByName);
        Console.WriteLine($"citySets.Count {citySets.Count}");
        
        var numberOfCitiesAdded = await InsertCitiesIntoDb( citySets);

        return new JsonResult(new
        {
            Cities = numberOfCitiesAdded,
            Countries = numberOfCountriesAdded
        });
    }

    [HttpGet]
    public async Task<ActionResult> CreateDefaultUsers()
    {
        throw new NotImplementedException();
    }

    private async Task<int> InsertCitiesIntoDb( HashSet<CitySet> citySets)
    {
        int numberOfCitiesAdded = 0;
        var cities = _context.Cities
                    .AsNoTracking()
                    .ToDictionary(x => (
                    Name: x.Name,
                    Lat: x.Lat,
                    Lon: x.Lon,
                    CountryId: x.CountryId));

        foreach (var item in citySets)
        {
            if (cities.ContainsKey((
               Name: item.Name,
               Lat: item.Lat,
               Lon: item.Lon,
               CountryId: item.CountryId
               )))
            {
                continue;
            }

            var city = new City
            {
                CountryId = item.CountryId,
                Lat = item.Lat,
                Lon = item.Lon,
                Name = item.Name
            };
            _context.Cities.Add(city);
            numberOfCitiesAdded++;
        }

        if (numberOfCitiesAdded > 0)
        {
            await _context.SaveChangesAsync();
        }

        return numberOfCitiesAdded;
    }

    private static HashSet<CitySet> ExtractCities(ExcelWorksheet worksheet, int nEndRow, int nEndColumn, Dictionary<string, Country> countriesByName)
    {
        HashSet<CitySet> citySets = new HashSet<CitySet>();

        for (int nRow = 2; nRow <= nEndRow; nRow++)
        {
            var row = worksheet.Cells[
                nRow, 1, nRow, nEndColumn
                ];

            var name = row[nRow, 1].GetValue<string>();
            var lat = row[nRow, 3].GetValue<decimal>();
            var lon = row[nRow, 4].GetValue<decimal>();
            var countryName = row[nRow, 5].GetValue<string>();
            var countryId = countriesByName[countryName].Id;
            citySets.Add(new CitySet
            {
                CountryId = countryId,
                Lat = lat,
                Lon = lon,
                Name = name
            });
        }

        return citySets;
    }

    private static HashSet<CountrySet> ExtractCountries(ExcelWorksheet worksheet, int nEndRow, int nEndColumn)
    {
        HashSet<CountrySet> countries = new HashSet<CountrySet>();

        for (int nRow = 2; nRow <= nEndRow; nRow++)
        {
            var row = worksheet.Cells[
                nRow, 1, nRow, nEndColumn
                ];
            var countryName = row[nRow, 5].GetValue<string>();
            var iso2 = row[nRow, 6].GetValue<string>();
            var iso3 = row[nRow, 7].GetValue<string>();
            countries.Add(new CountrySet
            {
                ISO2 = iso2,
                ISO3 = iso3,
                Name = countryName,
            });
        }

        return countries;
    }

    private async Task<int> InsertCountriesIntoDb( Dictionary<string, Country> countriesByName, HashSet<CountrySet> countries)
    {
        int numberOfCountriesAdded = 0;
        foreach (var item in countries)
        {
            var countryName = item.Name;
            if (countriesByName.ContainsKey(countryName))
            {
                continue;
            }
            var country = new Country
            {
                Name = item.Name,
                ISO2 = item.ISO2,
                ISO3 = item.ISO3,
            };
            // add the new country to the DB context
            await _context.Countries.AddAsync(country);

            // store the country in our lookup to retrieve its ID later on
            countriesByName.Add(countryName, country);

            numberOfCountriesAdded++;

        }


        // save all the countries into the database
        if (numberOfCountriesAdded > 0)
        {
            await _context.SaveChangesAsync();
        }
        return numberOfCountriesAdded;


    }

    class CitySet
    {
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// City Latitude
        /// </summary>
        [Column(TypeName = "decimal(7,4)")]
        public decimal Lat { get; set; }

        /// <summary>
        /// City Lognitude
        /// </summary>
        [Column(TypeName = "decimal(7,4)")]
        public decimal Lon { get; set; }


        public int CountryId { get; set; }

        public override int GetHashCode()
        {
            return (Name + Lat +Lon + CountryId).GetHashCode();
        }
        public override bool Equals(object? obj)
        {
            var other = obj as CitySet;
            if (other == null) return false;
            return Name == other.Name && Lat == other.Lat && Lon == other.Lon && CountryId == other.CountryId;
        }
    }

    class CountrySet
    {
        public  string Name { get; set; } = string.Empty;
        public string ISO2 { get; set; } = string.Empty;
        public string ISO3 { get; set; } = string.Empty;
        public override int GetHashCode()
        {
            return (Name + ISO2 + ISO3).GetHashCode();
        }
        public override bool Equals(object? obj)
        {
            var other = (obj as CountrySet);
            if (other == null) return false;
            return (Name == other.Name && ISO2 == other.ISO2 && ISO3 == other.ISO3);
        }
    }
}