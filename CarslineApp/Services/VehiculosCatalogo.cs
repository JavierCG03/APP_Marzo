using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarslineApp.Services
{
    /// <summary>
    /// Catálogo local de vehículos: Marca → Modelo → Versiones.
    /// </summary>
    public static class VehiculosCatalogo
    {
        // ══════════════════════════════════════════════════════════════════════
        //  TIPOS DE CARROCERÍA
        // ══════════════════════════════════════════════════════════════════════
        public enum TipoCarroceria
        {
            HATCHBACK,
            SEDAN,
            SUV,
            MINIVAN,
            PICKUP,
            AUTOSTURBO
        }

        // ══════════════════════════════════════════════════════════════════════
        //  CATÁLOGO  Marca → Modelo → Lista de Versiones
        // ══════════════════════════════════════════════════════════════════════
        private static readonly Dictionary<string, Dictionary<string, List<string>>> _catalogo =
            new(StringComparer.OrdinalIgnoreCase)
            {
                // ─────────────────────────────────────────────────────────────────
                ["Chevrolet"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Agile"] = new() { "LS", "LT", "LTZ" },
                    ["Aveo"] = new() { "LS", "LT", "Premier" },
                    ["Beat"] = new() { "LS", "LT", "RS", "Premier" },
                    ["Cavalier"] = new() { "LS", "LT", "RS", "Premier" },
                    ["Cobalt"] = new() { "LS", "LT", "SS" },
                    ["Cruze"] = new() { "LS", "LT", "Premier", "RS" },
                    ["Epica"] = new() { "LS", "LT" },
                    ["Impala"] = new() { "LS", "LT", "Premier" },
                    ["Malibu"] = new() { "LS", "LT", "Premier", "RS" },
                    ["Monza"] = new() { "LS", "LT" },
                    ["Onix"] = new() { "LS", "LT", "RS", "Premier" },
                    ["Optra"] = new() { "LS", "LT" },
                    ["Prisma"] = new() { "LS", "LT" },
                    ["Sail"] = new() { "LS", "LT" },
                    ["Sonic"] = new() { "LS", "LT", "RS", "Premier" },
                    ["Spark"] = new() { "LS", "LT", "Premier", "Activ" },
                    ["Vectra"] = new() { "LS", "LT", "GT" },
                    ["Blazer"] = new() { "LT", "RS", "Premier", "AWD" },
                    ["Blazer EV"] = new() { "LT", "RS", "SS" },
                    ["Captiva"] = new() { "LS", "LT", "Premier" },
                    ["Equinox"] = new() { "LS", "LT", "RS", "Premier" },
                    ["Equinox EV"] = new() { "LT", "RS" },
                    ["Groove"] = new() { "LS", "LT", "Premier" },
                    ["Orlando"] = new() { "LS", "LT" },
                    ["Suburban"] = new() { "LS", "LT", "RST", "Z71", "Premier", "High Country" },
                    ["Tahoe"] = new() { "LS", "LT", "RST", "Z71", "Premier", "High Country" },
                    ["Tracker"] = new() { "LS", "LT", "Premier", "RS" },
                    ["Trailblazer"] = new() { "LS", "LT", "RS", "Activ" },
                    ["Traverse"] = new() { "LS", "LT", "RS", "Premier", "High Country" },
                    ["Trax"] = new() { "LS", "LT", "RS", "Activ", "Premier" },
                    ["Avalanche"] = new() { "LS", "LT", "LTZ" },
                    ["Colorado"] = new() { "WT", "LT", "Z71", "ZR2" },
                    ["LUV"] = new() { "LS", "LT" },
                    ["Montana"] = new() { "LS", "LT", "Premier", "RS" },
                    ["S10"] = new() { "LS", "LT", "LTZ", "High Country", "Z71" },
                    ["Silverado"] = new() { "WT", "Custom", "LT", "RST", "LTZ", "High Country", "ZR2" },
                    ["Silverado EV"] = new() { "WT", "RST" },
                    ["Tornado"] = new() { "LS", "LT" },
                    ["Camaro"] = new() { "LS", "LT", "SS", "ZL1" },
                    ["Corvette"] = new() { "Stingray", "Z06", "ZR1", "E-Ray" },
                    ["Bolt EV"] = new() { "LT", "Premier" },
                    ["Bolt EUV"] = new() { "LT", "Premier" },
                    ["HHR"] = new() { "LS", "LT" },
                    ["SSR"] = new() { "Base" },
                    ["Uplander"] = new() { "LS", "LT" },
                    ["Venture"] = new() { "LS", "LT" },
                },
                // ─────────────────────────────────────────────────────────────────
                ["Ford"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Fiesta"] = new() { "S", "SE", "Titanium", "ST" },
                    ["Focus"] = new() { "S", "SE", "SEL", "Titanium", "ST", "RS" },
                    ["Fusion"] = new() { "S", "SE", "SEL", "Titanium", "Sport", "Energi" },
                    ["Taurus"] = new() { "SE", "SEL", "Limited", "SHO" },
                    ["Mondeo"] = new() { "Trend", "Titanium", "ST-Line" },
                    ["Ka"] = new() { "S", "SE", "Titanium" },
                    ["Ikon"] = new() { "Base", "Ambiente", "Trend" },
                    ["Crown Victoria"] = new() { "Base", "LX", "Police Interceptor" },
                    ["Bronco"] = new() { "Base", "Big Bend", "Black Diamond", "Badlands", "Outer Banks", "Wildtrak", "Raptor" },
                    ["Bronco Sport"] = new() { "Base", "Big Bend", "Outer Banks", "Badlands" },
                    ["EcoSport"] = new() { "S", "SE", "Titanium", "Storm" },
                    ["Edge"] = new() { "SE", "SEL", "ST", "Titanium" },
                    ["Escape"] = new() { "S", "SE", "SEL", "Titanium", "ST-Line", "Hybrid", "PHEV" },
                    ["Everest"] = new() { "Trend", "Sport", "Titanium", "Platinum" },
                    ["Explorer"] = new() { "Base", "XLT", "Limited", "ST", "Timberline", "Platinum" },
                    ["Expedition"] = new() { "XL", "XLT", "Limited", "Timberline", "King Ranch", "Platinum" },
                    ["Kuga"] = new() { "Trend", "Titanium", "ST-Line" },
                    ["Puma"] = new() { "Titanium", "ST-Line" },
                    ["Territory"] = new() { "Trend", "SEL", "Titanium" },
                    ["Maverick"] = new() { "XL", "XLT", "Lariat", "Tremor" },
                    ["Ranger"] = new() { "XL", "XLT", "Lariat", "Tremor", "Raptor" },
                    ["F-150"] = new() { "XL", "XLT", "Lariat", "King Ranch", "Platinum", "Limited", "Raptor" },
                    ["F-150 Lightning"] = new() { "Pro", "XLT", "Lariat", "Platinum" },
                    ["F-250"] = new() { "XL", "XLT", "Lariat", "King Ranch", "Platinum", "Limited" },
                    ["F-350"] = new() { "XL", "XLT", "Lariat", "King Ranch", "Platinum", "Limited" },
                    ["F-450"] = new() { "XL", "XLT", "Lariat", "Platinum" },
                    ["Courier"] = new() { "Base", "XL", "XLT" },
                    ["Mustang"] = new() { "EcoBoost", "EcoBoost Premium", "GT", "GT Premium", "Dark Horse", "Mach 1", "Shelby GT350", "Shelby GT500" },
                    ["Mustang Mach-E"] = new() { "Select", "Premium", "GT", "Rally" },
                    ["Ford GT"] = new() { "Base" },
                    ["Transit"] = new() { "Cargo", "Passenger", "Crew" },
                    ["Transit Connect"] = new() { "XL", "XLT", "Titanium" },
                    ["Tourneo"] = new() { "Trend", "Titanium" },
                },
                // ─────────────────────────────────────────────────────────────────
                ["Nissan"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    // ── Sedanes ─────────────────────────────────────────────────────────
                    ["Altima"] = new() { "S", "SV", "SR", "SL", "Platinum" },
                    ["Sentra"] = new() { "S", "SV", "SR", "SR Midnight", "SL" },
                    ["Versa"] = new() { "S", "SV", "SR", "Advance", "Exclusive" },
                    ["Tiida"] = new() { "Sense", "Advance", "Exclusive" },
                    ["Maxima"] = new() { "S", "SV", "SR", "Platinum" },
                    ["Sylphy"] = new() { "Sense", "Advance", "Exclusive" },
                    ["Almera"] = new() { "S", "SV", "SR" },
                    ["Sunny"] = new() { "S", "SV", "SL" },

                    // ── Hatchbacks ──────────────────────────────────────────────────────
                    ["March"] = new() { "Sense", "Advance", "SR", "Nismo" },
                    ["Versa Note"] = new() { "S", "SV", "SR" },
                    ["Micra"] = new() { "Visia", "Acenta", "N-Sport", "Tekna" },
                    ["Note"] = new() { "Sense", "Advance", "Exclusive" },
                    ["Leaf"] = new() { "S", "SV", "SV Plus", "SL Plus" },

                    // ── SUVs / CUVs ─────────────────────────────────────────────────────
                    ["Juke"] = new() { "S", "SV", "SL", "Nismo" },
                    ["Kicks"] = new() { "Sense", "Advance", "Exclusive", "SR" },
                    ["Kicks Play"] = new() { "Sense", "Advance" },
                    ["Qashqai"] = new() { "Sense", "Advance", "Exclusive" },
                    ["Rogue"] = new() { "S", "SV", "SL", "Platinum" },
                    ["Rogue Sport"] = new() { "S", "SV", "SL" },
                    ["X-Trail"] = new() { "Sense", "Advance", "Exclusive", "Platinum" },
                    ["Murano"] = new() { "S", "SV", "SL", "Platinum" },
                    ["Pathfinder"] = new() { "S", "SV", "SL", "Rock Creek", "Platinum" },
                    ["Armada"] = new() { "SV", "SL", "Midnight Edition", "Platinum" },
                    ["Terrano"] = new() { "Sense", "Advance", "Exclusive" },
                    ["Ariya"] = new() { "Engage", "Evolve", "Empower", "Platinum+" },
                    ["Ariya e-4ORCE"] = new() { "Engage", "Evolve", "Platinum+" },
                    ["Patrol"] = new() { "SE", "XE", "LE", "Platinum" },
                    ["Xterra"] = new() { "S", "SE", "PRO-4X" },

                    // ── Pickups ─────────────────────────────────────────────────────────
                    ["Frontier"] = new() { "S", "SV", "PRO-4X", "Pro-X", "Platinum" },
                    ["NP300"] = new() { "SE", "XE", "LE", "Platinum", "Pro-4X" },
                    ["Navara"] = new() { "S", "SE", "LE", "Pro-4X" },
                    ["Titan"] = new() { "S", "SV", "PRO-4X", "Platinum Reserve" },

                    // ── Deportivos ──────────────────────────────────────────────────────
                    ["370Z"] = new() { "Base", "Sport", "Nismo" },
                    ["400Z"] = new() { "Sport", "Performance", "Nismo" },
                    ["GT-R"] = new() { "Premium", "Track Edition", "Nismo" },

                    // ── Minivans / Vans ─────────────────────────────────────────────────
                    ["Urvan"] = new() { "Panel", "Panel Ventanas", "11 Pasajeros", "14 Pasajeros" },
                    ["NV200"] = new() { "S", "SV" },
                    ["NV350"] = new() { "Panel", "Pasajeros" },
                },
                // ─────────────────────────────────────────────────────────────────
                ["Toyota"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    // ── Sedanes ─────────────────────────────────────────────────────────
                    ["Camry"] = new() { "LE", "SE", "XLE", "XSE", "TRD" },
                    ["Corolla"] = new() { "Base CVT", "LE CVT", "XLE CVT", "SE CVT", "LE HEV", "XLE HEV" },
                    ["Corolla Cross"] = new() { "S", "SE", "XLE", "Hybrid LE", "Hybrid XLE" },
                    ["Prius"] = new() { "LE", "XLE", "Limited", "Prime LE", "Prime XLE" },                    
                    ["Yaris Sedan"] = new() { "S", "S CVT", "GR Sport" },

                    // ── Hatchbacks ──────────────────────────────────────────────────────
                    ["Yaris Hatchback"] = new() { "S", "S CVT", "GR Sport", "GR" },
                    ["Yaris GR"] = new() { "Circuit", "Rally" },
                    ["Prius C"] = new() { "Base", "L", "LE", "XLE" },
                    ["GR86"] = new() { "Base", "Premium" },

                    // ── CUVs / SUVs ─────────────────────────────────────────────────────
                    ["4Runner"] = new() { "SR5", "TRD Off-Road", "TRD Pro", "Limited", "Venture" },
                    ["bZ4X"] = new() { "XLE", "Limited" },
                    ["C-HR"] = new() { "LE", "XLE", "Limited" },
                    ["Crown"] = new() { "XLE", "Limited", "Platinum" },
                    ["Fortuner"] = new() { "SR", "SR5", "SRV", "GR-S" },
                    ["Land Cruiser"] = new() { "GX", "VX", "GR Sport" },
                    ["Land Cruiser Prado"] = new() { "TX", "TXL", "VX", "GR Sport" },
                    ["RAV4"] = new() { "LE", "XLE", "XLE Premium", "Adventure", "Limited", "Hybrid LE", "Hybrid XLE", "Hybrid Limited" },
                    ["Sequoia"] = new() { "SR5", "Limited", "Platinum", "TRD Pro", "Capstone" },
                    ["Venza"] = new() { "LE", "XLE", "Limited" },
                    ["Raize"] = new() { "X MT", "X CVT", "G CVT", "Z CVT" },
                    // ── Pickups ─────────────────────────────────────────────────────────
                    ["Hilux"] = new() { "DX", "SR", "SRV", "GR-S" },
                    ["Tacoma"] = new() { "SR", "SR5", "TRD Sport", "TRD Off-Road", "Limited", "TRD Pro" },
                    ["Tundra"] = new() { "Limited", "Platinum", "TRD Off Road HEV", "TRD Pro HEV" },

                    // ── Minivans / Vans ─────────────────────────────────────────────────
                    ["Hiace"] = new() { "Panel", "Panel Ventanas", "Panel Superlarga", "Ventanas Superlarga", "12 Pasajeros" },
                    ["Sienna"] = new() { "LE", "XLE", "XSE", "Limited", "Platinum" },
                },
                // ─────────────────────────────────────────────────────────────────
                ["Lexus"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["UX"] = new() { "UX 200", "UX 250h" },
                    ["IS"] = new() { "IS 300", "IS 350 F SPORT" },
                    ["ES"] = new() { "ES 250", "ES 300h", "ES 350" },
                    ["NX"] = new() { "NX 250", "NX 350", "NX 350h", "NX 450h+" },
                    ["RX"] = new() { "RX 350", "RX 350h", "RX 500h F SPORT" },
                    ["TX"] = new() { "TX 350h", "TX 500h F SPORT" },
                    ["GX"] = new() { "GX 460", "GX 550 Overtrail", "GX 550 Overtrail Plus" },
                    ["LX"] = new() { "LX 600", "LX 700h Luxury" },
                    ["LC"] = new() { "LC 500", "LC 500h" },
                    ["LS"] = new() { "LS 500", "LS 500h" },
                    ["RZ"] = new() { "RZ 450e", "RZ 550e F SPORT" },
                },
                // ─────────────────────────────────────────────────────────────────
                ["Volkswagen"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    // ── Hatchbacks / Compactos ───────────────────────────────────────────
                    ["Beetle"] = new() { "Design", "Sport", "R-Line", "Final Edition" },
                    ["Crossfox"] = new() { "Trendline", "Comfortline" },
                    ["Fox"] = new() { "Trendline", "Comfortline" },
                    ["Gol"] = new() { "Trendline", "Comfortline" },
                    ["Golf"] = new() { "Trendline", "Comfortline", "Highline", "GTI", "R" },
                    ["Golf GTI"] = new() { "Base", "Autobahn" },
                    ["Golf R"] = new() { "Base" },
                    ["Lupo"] = new() { "Trendline", "Comfortline" },
                    ["Polo"] = new() { "Trendline", "Comfortline", "Highline" },
                    ["Up!"] = new() { "Take Up!", "Move Up!", "High Up!" },

                    // ── Sedanes ─────────────────────────────────────────────────────────
                    ["Bora"] = new() { "Trendline", "Comfortline", "Highline", "GLI" },
                    ["Jetta"] = new() { "Trendline", "Comfortline", "Highline", "GLI", "SEL" },
                    ["Passat"] = new() { "Trendline", "Comfortline", "Highline" },
                    ["Phaeton"] = new() { "V6", "W12" },
                    ["Vento"] = new() { "Trendline", "Comfortline", "Highline", "GLI" },
                    ["Virtus"] = new() { "Trendline", "Comfortline", "Highline" },

                    // ── CUVs / SUVs ─────────────────────────────────────────────────────
                    ["Taigun"] = new() { "Trendline", "Comfortline", "Highline" },
                    ["Taos"] = new() { "Trendline", "Comfortline", "Highline" },
                    ["T-Cross"] = new() { "Trendline", "Comfortline", "Highline" },
                    ["T-Roc"] = new() { "Trendline", "Comfortline", "Highline", "R-Line" },
                    ["Tiguan"] = new() { "Trendline", "Comfortline", "Highline", "R-Line" },
                    ["Tiguan Allspace"] = new() { "Trendline", "Comfortline", "Highline" },
                    ["Teramont"] = new() { "Trendline", "Comfortline", "Highline" },
                    ["Touareg"] = new() { "Premium", "R-Line", "Elegance" },

                    // ── Pickups ─────────────────────────────────────────────────────────
                    ["Amarok"] = new() { "Trendline", "Comfortline", "Highline", "Aventura", "V6 Highline", "V6 Aventura" },
                    ["Saveiro"] = new() { "Trendline", "Comfortline", "Cross" },

                    // ── Minivans / Vans ─────────────────────────────────────────────────
                    ["Caravelle"] = new() { "Trendline", "Comfortline", "Highline" },
                    ["Multivan"] = new() { "Trendline", "Comfortline", "Highline" },
                    ["Routan"] = new() { "SE", "SEL" },
                    ["Sharan"] = new() { "Trendline", "Comfortline", "Highline" },
                    ["Touran"] = new() { "Trendline", "Comfortline", "Highline" },
                    ["Transporter"] = new() { "Panel", "Mixto", "9 Pasajeros" },

                    // ── Eléctricos ──────────────────────────────────────────────────────
                    ["ID.3"] = new() { "Pure", "Life", "Style", "Max" },
                    ["ID.4"] = new() { "Pure", "Life", "Style", "GTX" },
                    ["ID.5"] = new() { "Style", "GTX" },
                    ["ID.6"] = new() { "Pure", "Life", "Style" },
                    ["ID.7"] = new() { "Pro", "Pro S" },
                    ["ID. Buzz"] = new() { "Pro", "GTX" },

                    // ── Deportivos ──────────────────────────────────────────────────────
                    ["Arteon"] = new() { "Elegance", "R-Line", "R" },
                    ["CC"] = new() { "Trendline", "Comfortline", "Highline" },
                    ["Corrado"] = new() { "VR6", "G60" },
                    ["Scirocco"] = new() { "Trendline", "Comfortline", "R-Line", "R" },
                },
                // ─────────────────────────────────────────────────────────────────
                ["Honda"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Accord"] = new() { "LX", "Sport", "EX-L", "Touring", "Hybrid" },
                    ["BR-V"] = new() { "Prime", "Uniq", "Epic" },
                    ["City"] = new() { "Uniq", "Prime", "Epic", "Sport" },
                    ["Civic"] = new() { "LX", "Sport", "EX", "EX-L", "Touring", "Si", "Type R" },
                    ["CR-V"] = new() { "Turbo", "Turbo Plus", "Touring", "Hybrid" },
                    ["Fit"] = new() { "Fun", "Hit", "EX" },
                    ["HR-V"] = new() { "Prime", "Epic", "Touring" },
                    ["Insight"] = new() { "EX", "Touring" },
                    ["WR-V"] = new() { "Uniq", "Prime" },
                    ["CR-Z"] = new() { "Base", "EX" },
                    ["Crosstour"] = new() { "EX", "EX-L" },
                    ["Element"] = new() { "EX", "SC" },
                    ["Odyssey"] = new() { "LX", "EX", "EX-L", "Touring", "Elite" },
                    ["Pilot"] = new() { "Sport", "EX-L", "TrailSport", "Elite" },
                    ["Ridgeline"] = new() { "Sport", "RTL", "RTL-E", "Black Edition" },
                },
                // ─────────────────────────────────────────────────────────────────
                ["Hyundai"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Accent"] = new() { "GL", "GLS", "GLS Mid", "GLS Premium" },
                    ["Creta"] = new() { "GL", "GLS", "Limited" },
                    ["Elantra"] = new() { "GLS", "Limited", "N Line" },
                    ["Ioniq"] = new() { "Hybrid GLS", "Plug-in GLS", "Electric GLS" },
                    ["Kona"] = new() { "GLS", "Limited", "N Line", "Electric" },
                    ["Santa Fe"] = new() { "GLS", "Limited", "Limited 2.5T" },
                    ["Sonata"] = new() { "GLS", "Limited", "N Line" },
                    ["Tucson"] = new() { "GLS", "Limited", "N Line" },
                    ["Venue"] = new() { "GL", "GLS", "Limited" },
                },
                // ─────────────────────────────────────────────────────────────────
                ["Kia"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Carnival"] = new() { "LX", "EX", "SX", "SX Prestige" },
                    ["EV6"] = new() { "Wind", "GT-Line", "GT-Line AWD" },
                    ["K3"] = new() { "EX", "EX Premium", "GT Line" },
                    ["K4"] = new() { "L MT", "L IVT", "LX MT", "LX IVT", "EX IVT", "GT Line", "GT Line Turbo" },
                    ["K5"] = new() { "LX", "EX", "GT-Line", "EX Premium" },
                    ["Niro"] = new() { "LX Hybrid", "EX Hybrid", "EX Premium Hybrid", "EV LX", "EV EX" },
                    ["Rio"] = new() { "LX", "EX", "GT Line" },
                    ["Seltos"] = new() { "LX", "EX", "EX Premium", "SX" },
                    ["Sorento"] = new() { "LX", "EX", "SX", "SX Prestige" },
                    ["Soul"] = new() { "LX", "EX", "GT-Line", "Turbo" },
                    ["Sportage"] = new() { "LX", "EX", "EX Premium", "SX" },
                    ["Stinger"] = new() { "GT-Line", "GT1", "GT2" },
                },
                // ─────────────────────────────────────────────────────────────────
                ["Mazda"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["CX-3"] = new() { "i Sport", "i Grand Touring", "s Grand Touring" },
                    ["CX-30"] = new() { "i Sport", "i SkyActiv", "i Grand Touring", "s Grand Touring", "Turbo" },
                    ["CX-5"] = new() { "i Sport", "i SkyActiv", "i Grand Touring", "s Grand Touring", "Turbo", "Signature" },
                    ["CX-7"] = new() { "i Sport", "i Touring", "i Grand Touring" },
                    ["CX-9"] = new() { "Sport", "Touring", "Grand Touring", "Signature" },
                    ["CX-50"] = new() { "i Sport", "i Grand Touring", "Turbo", "Turbo Meridian Edition" },
                    ["CX-60"] = new() { "Prime-Line", "Exclusive-Line", "Homura", "Takumi" },
                    ["CX-70"] = new() { "Preferred", "Premium", "Premium Plus" },
                    ["CX-90"] = new() { "Preferred", "Premium", "Premium Plus", "Turbo S", "Turbo S Premium Plus" },
                    ["Mazda2"] = new() { "i Sport", "i Grand Touring", "Signature" },
                    ["Mazda3"] = new() { "i Sport", "i SkyActiv", "i Grand Touring", "s Grand Touring", "Turbo", "Signature" },
                    ["Mazda5"] = new() { "Sport", "Touring", "Grand Touring" },
                    ["Mazda6"] = new() { "i Sport", "i Grand Touring", "s Grand Touring", "Signature" },
                    ["MX-5"] = new() { "Sport", "Club", "Grand Touring", "RF Club", "RF Grand Touring" },
                    ["MX-30"] = new() { "e-SkyActiv", "Premium Plus" },
                    ["BT-50"] = new() { "4x2", "4x4", "Pro", "High" }
                },
                // ─────────────────────────────────────────────────────────────────
                ["Dodge"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Attitude"] = new() { "SE", "SXT", "Sport", "GT" },
                    ["Challenger"] = new() { "SXT", "GT", "R/T", "R/T Scat Pack", "SRT 392", "SRT Hellcat", "SRT Demon" },
                    ["Charger"] = new() { "SXT", "GT", "R/T", "Scat Pack", "SRT 392", "SRT Hellcat", "SRT Hellcat Redeye" },
                    ["Durango"] = new() { "SXT", "GT", "R/T", "Citadel", "SRT 392", "SRT Hellcat" },
                    ["Journey"] = new() { "SE", "SXT", "Crossroad", "GT" },
                    ["Neon"] = new() { "SE", "SXT", "GT", "Sport" },
                    ["Vision"] = new() { "SE", "SXT" },
                    ["Grand Caravan"] = new() { "SE", "SXT", "GT" },
                    ["Dakota"] = new() { "SLT", "Sport", "Laramie" },
                    ["Viper"] = new() { "SRT-10", "GTS", "ACR" },
                    ["Nitro"] = new() { "SE", "SXT", "SLT", "R/T" },
                    ["Caliber"] = new() { "SE", "SXT", "R/T", "SRT4" },
                    ["Avenger"] = new() { "SE", "SXT", "R/T" },
                    ["i10"] = new() { "GL", "GLS" },
                },
                // ─────────────────────────────────────────────────────────────────
                ["Jeep"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Cherokee"] = new() { "Sport", "Latitude", "Trailhawk", "Limited", "Overland" },
                    ["Compass"] = new() { "Sport", "Latitude", "Trailhawk", "Limited" },
                    ["Gladiator"] = new() { "Sport", "Sport S", "Overland", "Mojave", "Rubicon" },
                    ["Grand Cherokee"] = new() { "Laredo", "Altitude", "Limited", "Trailhawk", "Overland", "Summit" },
                    ["Renegade"] = new() { "Sport", "Latitude", "Trailhawk", "Limited" },
                    ["Wrangler"] = new() { "Sport", "Sport S", "Sahara", "Rubicon", "4xe" },
                },
                // ─────────────────────────────────────────────────────────────────
                ["RAM"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["700"] = new() { "SLT Cabina Regular", "SLT Cabina Doble", "Big Horn", "Big Horn CVT", "Laramie", "Laramie CVT Turbo" },
                    ["1200"] = new() { "Tradesman Regular Cab", "Tradesman Doble Cab", "Big Horn", "Laramie" },
                    ["1500"] = new() { "Tradesman", "Express", "Big Horn", "Rebel", "Laramie", "Longhorn", "Limited", "TRX", "RHO" },
                    ["2500"] = new() { "Tradesman", "Big Horn", "Power Wagon", "Laramie", "Longhorn", "Limited" },
                    ["ProMaster"] = new() { "1500 Cargo Van", "2500 Cargo Van", "3500 Cargo Van", "2500 Window Van", "3500 Window Van", "City Cargo Van", "City Wagon" },
                    ["3500"] = new() { "Tradesman", "Big Horn", "Laramie", "Power Wagon", "Limited Longhorn", "Limited" },
                },
                // ─────────────────────────────────────────────────────────────────
                ["Audi"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["A1"] = new() { "30 TFSI", "35 TFSI", "S1" },
                    ["A3"] = new() { "35 TFSI", "40 TFSI", "45 TFSI e", "S3" },
                    ["A4"] = new() { "35 TFSI", "40 TFSI", "45 TFSI", "S4" },
                    ["A5"] = new() { "40 TFSI", "45 TFSI", "S5" },
                    ["A6"] = new() { "40 TFSI", "45 TFSI", "55 TFSI", "S6" },
                    ["Q2"] = new() { "35 TFSI", "40 TFSI" },
                    ["Q3"] = new() { "35 TFSI", "40 TFSI", "45 TFSI e", "RS Q3" },
                    ["Q5"] = new() { "40 TFSI", "45 TFSI", "55 TFSI e", "SQ5" },
                    ["Q7"] = new() { "45 TFSI", "55 TFSI", "SQ7" },
                    ["Q8"] = new() { "55 TFSI", "SQ8", "RS Q8", "e-tron" },
                    ["TT"] = new() { "40 TFSI", "45 TFSI", "TTS" },
                },
                // ─────────────────────────────────────────────────────────────────
                ["BMW"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["116i"] = new() { "Advantage", "Sport Line", "M Sport" },
                    ["118i"] = new() { "Advantage", "Sport Line", "M Sport" },
                    ["218i"] = new() { "Advantage", "Sport Line", "M Sport" },
                    ["320i"] = new() { "Advantage", "Sport Line", "M Sport", "xDrive" },
                    ["330i"] = new() { "M Sport", "xDrive", "M Sport xDrive" },
                    ["420i"] = new() { "Advantage", "Sport Line", "M Sport" },
                    ["520i"] = new() { "Advantage", "Sport Line", "M Sport" },
                    ["530i"] = new() { "M Sport", "xDrive M Sport" },
                    ["X1"] = new() { "sDrive18i", "sDrive20i", "xDrive20i M Sport" },
                    ["X2"] = new() { "sDrive18i", "sDrive20i", "xDrive20i M Sport" },
                    ["X3"] = new() { "sDrive20i", "xDrive20i", "xDrive30i", "M40i" },
                    ["X4"] = new() { "xDrive20i", "xDrive30i", "M40i" },
                    ["X5"] = new() { "xDrive40i", "xDrive45e", "M50i", "xDrive50e" },
                    ["X6"] = new() { "xDrive40i", "M50i" },
                },
                // ─────────────────────────────────────────────────────────────────
                ["Mercedes-Benz"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["A 200"] = new() { "Progressive", "AMG Line", "AMG" },
                    ["B 200"] = new() { "Progressive", "AMG Line" },
                    ["C 180"] = new() { "Avantgarde", "AMG Line" },
                    ["C 200"] = new() { "Avantgarde", "AMG Line", "AMG" },
                    ["CLA"] = new() { "200 Progressive", "200 AMG Line", "AMG 35" },
                    ["E 200"] = new() { "Avantgarde", "AMG Line" },
                    ["GLA"] = new() { "200 Progressive", "200 AMG Line", "AMG 35" },
                    ["GLB"] = new() { "200 Progressive", "200 AMG Line", "AMG 35" },
                    ["GLC"] = new() { "300 Avantgarde", "300 AMG Line", "AMG 43", "AMG 63" },
                    ["GLE"] = new() { "350 Avantgarde", "350 AMG Line", "AMG 53", "AMG 63" },
                    ["GLS"] = new() { "450 AMG Line", "AMG 63" },
                },
                // ─────────────────────────────────────────────────────────────────
                ["Peugeot"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["208"] = new() { "Active", "Allure", "GT Line", "GT" },
                    ["2008"] = new() { "Active", "Allure", "GT Line", "GT" },
                    ["3008"] = new() { "Active", "Allure", "GT Line", "GT" },
                    ["308"] = new() { "Active", "Allure", "GT Line", "GT" },
                    ["408"] = new() { "Active", "Allure", "GT Line" },
                    ["5008"] = new() { "Active", "Allure", "GT Line", "GT" },
                },
                // ─────────────────────────────────────────────────────────────────
                ["Renault"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Captur"] = new() { "Zen", "Intens", "Iconic" },
                    ["Duster"] = new() { "Zen", "Intens", "Iconic", "Outsider" },
                    ["Kangoo"] = new() { "Express", "Confort" },
                    ["Koleos"] = new() { "Zen", "Intens", "Iconic" },
                    ["Logan"] = new() { "Zen", "Intens" },
                    ["Oroch"] = new() { "Zen", "Intens", "Outsider" },
                    ["Sandero"] = new() { "Zen", "Stepway Zen", "Stepway Intens" },
                    ["Stepway"] = new() { "Zen", "Intens", "Iconic" },
                },
                // ─────────────────────────────────────────────────────────────────
                ["SEAT"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Arona"] = new() { "Reference", "Style", "FR", "Xcellence" },
                    ["Ateca"] = new() { "Reference", "Style", "FR", "Xcellence" },
                    ["Ibiza"] = new() { "Reference", "Style", "FR", "Xcellence" },
                    ["Leon"] = new() { "Reference", "Style", "FR", "Xcellence", "Cupra", "FR Black Edition" },
                    ["Leon Sportstourer"] = new() { "Style", "FR", "Xcellence" },
                    ["Tarraco"] = new() { "Style", "FR", "Xcellence" },
                    ["Toledo"] = new() { "Reference", "Style", "FR", "Advance" },
                    ["Altea"] = new() { "Reference", "Style", "XL", "FR" },
                    ["Altea XL"] = new() { "Reference", "Style", "FR" },
                    ["Exeo"] = new() { "Reference", "Style", "Sport" },
                    ["Mii"] = new() { "Reference", "Style", "FR" },
                    ["Cordoba"] = new() { "Reference", "Style", "Sport" },
                    ["Freetrack"] = new() { "Style", "4Drive" }
                },
                // ─────────────────────────────────────────────────────────────────
                ["CUPRA"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Formentor"] = new() { "1.4 e-Hybrid", "2.0 TSI 190", "2.0 TSI 245", "VZ 310" },
                    ["Leon"] = new() { "1.4 e-Hybrid", "2.0 TSI 245", "VZ 300" },
                    ["Ateca"] = new() { "2.0 TSI 300" },
                    ["Born"] = new() { "58 kWh", "77 kWh" },
                },
                // ─────────────────────────────────────────────────────────────────
                ["Subaru"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["BRZ"] = new() { "Premium", "Limited", "tS" },
                    ["Crosstrek"] = new() { "Base", "Premium", "Limited", "Sport", "Wilderness" },
                    ["Forester"] = new() { "Base", "Premium", "Sport", "Limited", "Touring", "Wilderness" },
                    ["Impreza"] = new() { "Base", "Premium", "Sport", "Limited" },
                    ["Legacy"] = new() { "Base", "Premium", "Sport", "Limited", "Touring XT" },
                    ["Outback"] = new() { "Base", "Premium", "Onyx Edition", "Limited", "Touring", "Wilderness" },
                    ["WRX"] = new() { "Base", "Premium", "Limited", "GT", "STI" },
                },
                // ─────────────────────────────────────────────────────────────────
                ["Mitsubishi"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Eclipse Cross"] = new() { "ES", "SE", "SEL", "SEL Premium" },
                    ["L200"] = new() { "GLX", "GLS", "HPE", "Athlete" },
                    ["Mirage"] = new() { "ES", "LE", "SE", "SEL" },
                    ["Montero"] = new() { "Sport ES", "Sport SE", "Sport SP" },
                    ["Outlander"] = new() { "ES", "SE", "SEL", "PHEV SEL" },
                },
                // ─────────────────────────────────────────────────────────────────
                ["Fiat"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Argo"] = new() { "Drive", "Trekking", "Precision" },
                    ["Cronos"] = new() { "Drive", "Precision" },
                    ["Mobi"] = new() { "Like", "Easy", "Trekking" },
                    ["Pulse"] = new() { "Drive", "Audace", "Impetus", "Abarth" },
                    ["Toro"] = new() { "Endurance", "Freedom", "Volcano", "Ultra" },
                },
                // ─────────────────────────────────────────────────────────────────
                ["Suzuki"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Baleno"] = new() { "GL", "GLX" },
                    ["Ignis"] = new() { "GL", "GLX" },
                    ["Jimny"] = new() { "GL", "GLX", "Sierra" },
                    ["Swift"] = new() { "GL", "GLX", "Sport" },
                    ["Vitara"] = new() { "GL", "GLX", "S" },
                },
                // ─────────────────────────────────────────────────────────────────
                ["Volvo"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["S60"] = new() { "Momentum", "R-Design", "Inscription", "T8 Polestar" },
                    ["S90"] = new() { "Momentum", "Inscription", "T8" },
                    ["V60"] = new() { "Momentum", "R-Design", "Inscription", "T8" },
                    ["XC40"] = new() { "Momentum", "R-Design", "Inscription", "Recharge" },
                    ["XC60"] = new() { "Momentum", "R-Design", "Inscription", "T8", "Recharge" },
                    ["XC90"] = new() { "Momentum", "R-Design", "Inscription", "T8 Excellence" },
                },
                // ─────────────────────────────────────────────────────────────────
                ["GMC"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Acadia"] = new() { "SLE", "SLT", "AT4", "Denali" },
                    ["Canyon"] = new() { "Base", "SLE", "Elevation", "SLT", "AT4", "Denali" },
                    ["Sierra"] = new() { "Base", "SLE", "SLT", "Elevation", "AT4", "Denali", "AT4X" },
                    ["Terrain"] = new() { "SLE", "SLT", "AT4", "Denali" },
                    ["Yukon"] = new() { "SLE", "SLT", "AT4", "Denali", "XL SLT", "XL Denali" },
                },
                // ─────────────────────────────────────────────────────────────────
                ["Buick"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Enclave"] = new() { "Preferred", "Essence", "Avenir" },
                    ["Encore"] = new() { "Preferred", "Sport Touring", "Essence", "Avenir" },
                    ["Envision"] = new() { "Preferred", "Essence", "Avenir" },
                },
                // ─────────────────────────────────────────────────────────────────
                ["Chrysler"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["300"] = new() { "Touring", "300S", "C", "SRT 392" },
                    ["Pacifica"] = new() { "Touring", "Touring L", "Limited", "Pinnacle", "Hybrid" },
                },
                // ─────────────────────────────────────────────────────────────────
                ["Mini"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Clubman"] = new() { "One", "Cooper", "Cooper S", "John Cooper Works" },
                    ["Cooper"] = new() { "One", "Cooper", "Cooper S", "John Cooper Works", "SE Electric" },
                    ["Countryman"] = new() { "One", "Cooper", "Cooper S", "Cooper SE ALL4", "John Cooper Works" },
                    ["Paceman"] = new() { "Cooper", "Cooper S", "John Cooper Works" },
                },
                // ─────────────────────────────────────────────────────────────────
                ["Porsche"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["718"] = new() { "Boxster", "Cayman", "Spyder", "GT4", "GTS 4.0" },
                    ["911"] = new() { "Carrera", "Carrera S", "Carrera 4S", "Targa 4S", "Turbo", "Turbo S", "GT3" },
                    ["Cayenne"] = new() { "Base", "S", "GTS", "Turbo", "Turbo S E-Hybrid" },
                    ["Macan"] = new() { "Base", "S", "GTS", "Turbo" },
                    ["Panamera"] = new() { "Base", "4", "4S", "GTS", "Turbo", "Turbo S E-Hybrid" },
                    ["Taycan"] = new() { "Base", "4S", "GTS", "Turbo", "Turbo S", "Cross Turismo" },
                },
                // ─────────────────────────────────────────────────────────────────
                ["Land Rover"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Defender"] = new() { "90 S", "90 SE", "90 HSE", "110 S", "110 SE", "110 HSE", "110 X" },
                    ["Discovery"] = new() { "S", "SE", "HSE", "R-Dynamic HSE", "Metropolitan" },
                    ["Discovery Sport"] = new() { "S", "SE", "HSE", "R-Dynamic SE", "R-Dynamic HSE" },
                    ["Freelander"] = new() { "SE", "HSE" },
                    ["Range Rover"] = new() { "SE", "HSE", "Autobiography", "SV" },
                    ["Range Rover Evoque"] = new() { "S", "SE", "HSE", "R-Dynamic HSE" },
                    ["Range Rover Sport"] = new() { "SE", "HSE", "HSE Dynamic", "Autobiography", "SVR" },
                },
                // ─────────────────────────────────────────────────────────────────
                ["Tesla"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Model 3"] = new() { "RWD", "Long Range", "Performance" },
                    ["Model S"] = new() { "Long Range", "Plaid" },
                    ["Model X"] = new() { "Long Range", "Plaid" },
                    ["Model Y"] = new() { "RWD", "Long Range", "Performance" },
                },
                // ─────────────────────────────────────────────────────────────────
                ["MG"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["MG3"] = new() { "Style", "Excite" },
                    ["MG5"] = new() { "Style MT", "Excite CVT", "Elegance CVT" },
                    ["MG GT"] = new() { "Style", "Excite", "Alpha" },
                    ["ZS"] = new() { "Style", "Excite" },
                    ["ZS EV"] = new() { "Excite", "Elegance" },
                    ["HS"] = new() { "Excite", "Trophy" },
                    ["RX5"] = new() { "Style", "Elegance" },
                    ["Marvel R"] = new() { "Luxury", "Performance" },
                },
                // ─────────────────────────────────────────────────────────────────
                ["BYD"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Dolphin"] = new() { "Dynamic", "Premium" },
                    ["Dolphin Mini"] = new() { "Dynamic", "Premium" },
                    ["Seal"] = new() { "Dynamic", "Premium", "Performance AWD" },
                    ["Song Plus"] = new() { "DM-i", "EV" },
                    ["Yuan Plus"] = new() { "GL", "GS" },
                    ["Tang"] = new() { "EV AWD" },
                    ["Han"] = new() { "EV", "EV AWD" },
                },
                // ─────────────────────────────────────────────────────────────────
                ["Chirey"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Tiggo 2 Pro"] = new() { "Comfort", "Luxury" },
                    ["Tiggo 4 Pro"] = new() { "Comfort", "Luxury" },
                    ["Tiggo 7 Pro"] = new() { "Luxury", "Premium" },
                    ["Tiggo 8 Pro"] = new() { "Luxury", "Premium", "e+" },
                },
                // ─────────────────────────────────────────────────────────────────
                ["Omoda"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["O5"] = new() { "Life", "Unlimited" },
                    ["C5"] = new() { "Life", "Unlimited" },
                },
                // ─────────────────────────────────────────────────────────────────
                ["JAC"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["J7"] = new() { "Quantum", "Limited" },
                    ["J4"] = new() { "Comfort", "Smart" },
                    ["Sei2"] = new() { "Connect", "Limited" },
                    ["Sei3"] = new() { "Quantum", "Limited" },
                    ["Sei4 Pro"] = new() { "Connect", "Limited" },
                    ["Sei7 Pro"] = new() { "Quantum", "Limited" },
                    ["E10X"] = new() { "Cargo", "Passenger" },
                    ["E J7"] = new() { "Quantum" },
                    ["Frison T6"] = new() { "Flex", "Diesel" },
                    ["Frison T8"] = new() { "Diesel 4x4" },
                },
                // ─────────────────────────────────────────────────────────────────
                ["GWM"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Haval H6"] = new() { "Premium", "HEV" },
                    ["Haval Jolion"] = new() { "Premium", "Luxury" },
                    ["Tank 300"] = new() { "Luxury", "Premium" },
                    ["Ora 03"] = new() { "400 km", "500 km" },
                },
                // ─────────────────────────────────────────────────────────────────
                ["BAIC"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["X35"] = new() { "Fashion", "Luxury" },
                    ["X55"] = new() { "Fashion", "Luxury" },
                    ["BJ40"] = new() { "Champion", "Plus" },
                    ["EU5"] = new() { "EV" },
                },
                // ─────────────────────────────────────────────────────────────────
                ["Changan"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Alsvin"] = new() { "MT", "AT" },
                    ["CS35 Plus"] = new() { "Comfort", "Luxury" },
                    ["CS55 Plus"] = new() { "Luxury", "Premium" },
                    ["CS75 Pro"] = new() { "Luxury" },
                    ["UNI-T"] = new() { "Elite", "Limited" },
                    ["UNI-K"] = new() { "Elite", "Limited" },
                },
                // ─────────────────────────────────────────────────────────────────
                ["Jetour"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["X70"] = new() { "Comfort", "Luxury" },
                    ["X70 Plus"] = new() { "Luxury", "Premium" },
                    ["Dashing"] = new() { "Comfort", "Luxury" },
                    ["T2"] = new() { "Luxury", "Premium" },
                },
                // ─────────────────────────────────────────────────────────────────
                ["Lincoln"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Corsair"] = new() { "Premier FWD", "Reserve AWD", "Grand Touring PHEV" },
                    ["Nautilus"] = new() { "Premiere", "Reserve", "Black Label" },
                    ["Aviator"] = new() { "Reserve", "Black Label", "Grand Touring" },
                    ["Navigator"] = new() { "Premiere", "Reserve", "Black Label", "Navigator L Reserve", "Navigator L Black Label" },
                },
                // ─────────────────────────────────────────────────────────────────
                ["Smart"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Fortwo"] = new() { "Passion", "Pro", "Prime" },
                    ["Forfour"] = new() { "Passion", "Pro", "Prime" },
                    ["#1"] = new() { "Standard", "Premium", "Performance" },
                    ["#6"] = new() { "Base PHEV", "Premium PHEV" },
                },
                // ─────────────────────────────────────────────────────────────────
                ["Geely"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Coolray"] = new() { "1.5T GL", "1.5T GF", "1.5T GF Bitono" },
                    ["Emgrand"] = new() { "GL", "GLX", "Luxury" },
                    ["Cityray"] = new() { "Standard", "Luxury" },
                    ["Starray"] = new() { "Standard", "Premium", "Performance" },
                    ["EX2"] = new() { "Electric Standard", "Electric Premium" },
                    ["EX5"] = new() { "EM-i GL", "EM-i Premium", "EV Standard" },
                    ["Monjaro"] = new() { "Standard", "Luxury", "Premium" },
                    ["Okavango"] = new() { "Standard", "Luxury" },
                    ["GX3 Pro"] = new() { "Standard", "Luxury" },
                },
                // ─────────────────────────────────────────────────────────────────
                ["Acura"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["ILX"] = new() { "Standard", "A-Spec", "Tech" },
                    ["TLX"] = new() { "Standard", "A-Spec", "SH-AWD" },
                    ["RDX"] = new() { "Standard", "A-Spec", "Advance", "Type S" },
                    ["MDX"] = new() { "Standard", "A-Spec", "Advance", "Type S" },
                    ["ZDX"] = new() { "Base", "A-Spec", "Tech" },
                },
                // ─────────────────────────────────────────────────────────────────
                ["Infiniti"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["QX50"] = new() { "Pure", "Luxe", "Sensory" },
                    ["QX55"] = new() { "Pure", "Luxe", "Sensory" },
                    ["QX60"] = new() { "Pure", "Luxe", "Sensory" },
                    ["QX80"] = new() { "Sensory 4WD", "Autograph 4WD" },
                },
                // ─────────────────────────────────────────────────────────────────
                ["Isuzu"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["D-Max"] = new() { "Workhorse", "Space Cab", "Worker", "V-Cross", "Highlander", "X-Terrain" },
                    ["MU-X"] = new() { "Comfort", "Premium", "RS", "Ultimate" },
                },
            };

        // ══════════════════════════════════════════════════════════════════════
        //  CATÁLOGO DE TIPOS  Marca → Modelo → TipoCarroceria
        // ══════════════════════════════════════════════════════════════════════
        private static readonly Dictionary<string, Dictionary<string, TipoCarroceria>> _tipos =
            new(StringComparer.OrdinalIgnoreCase)
            {
                // ─── Chevrolet ────────────────────────────────────────────────
                ["Chevrolet"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Agile"] = TipoCarroceria.HATCHBACK,
                    ["Aveo"] = TipoCarroceria.SEDAN,
                    ["Beat"] = TipoCarroceria.HATCHBACK,
                    ["Cavalier"] = TipoCarroceria.SEDAN,
                    ["Cobalt"] = TipoCarroceria.SEDAN,
                    ["Cruze"] = TipoCarroceria.SEDAN,
                    ["Epica"] = TipoCarroceria.SEDAN,
                    ["Impala"] = TipoCarroceria.SEDAN,
                    ["Malibu"] = TipoCarroceria.SEDAN,
                    ["Monza"] = TipoCarroceria.SEDAN,
                    ["Onix"] = TipoCarroceria.HATCHBACK,
                    ["Optra"] = TipoCarroceria.SEDAN,
                    ["Prisma"] = TipoCarroceria.SEDAN,
                    ["Sail"] = TipoCarroceria.SEDAN,
                    ["Sonic"] = TipoCarroceria.HATCHBACK,
                    ["Spark"] = TipoCarroceria.HATCHBACK,
                    ["Vectra"] = TipoCarroceria.SEDAN,
                    ["Blazer"] = TipoCarroceria.SUV,
                    ["Blazer EV"] = TipoCarroceria.SUV,
                    ["Captiva"] = TipoCarroceria.SUV,
                    ["Equinox"] = TipoCarroceria.SUV,
                    ["Equinox EV"] = TipoCarroceria.SUV,
                    ["Groove"] = TipoCarroceria.SUV,
                    ["Orlando"] = TipoCarroceria.MINIVAN,
                    ["Suburban"] = TipoCarroceria.SUV,
                    ["Tahoe"] = TipoCarroceria.SUV,
                    ["Tracker"] = TipoCarroceria.SUV,
                    ["Trailblazer"] = TipoCarroceria.SUV,
                    ["Traverse"] = TipoCarroceria.SUV,
                    ["Trax"] = TipoCarroceria.SUV,
                    ["Avalanche"] = TipoCarroceria.PICKUP,
                    ["Colorado"] = TipoCarroceria.PICKUP,
                    ["LUV"] = TipoCarroceria.PICKUP,
                    ["Montana"] = TipoCarroceria.PICKUP,
                    ["S10"] = TipoCarroceria.PICKUP,
                    ["Silverado"] = TipoCarroceria.PICKUP,
                    ["Silverado EV"] = TipoCarroceria.PICKUP,
                    ["Tornado"] = TipoCarroceria.PICKUP,
                    ["Camaro"] = TipoCarroceria.AUTOSTURBO,
                    ["Corvette"] = TipoCarroceria.AUTOSTURBO,
                    ["Bolt EV"] = TipoCarroceria.HATCHBACK,
                    ["Bolt EUV"] = TipoCarroceria.SUV,
                    ["HHR"] = TipoCarroceria.HATCHBACK,
                    ["SSR"] = TipoCarroceria.AUTOSTURBO,
                    ["Uplander"] = TipoCarroceria.MINIVAN,
                    ["Venture"] = TipoCarroceria.MINIVAN,
                },
                // ─── Ford ─────────────────────────────────────────────────────
                ["Ford"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Fiesta"] = TipoCarroceria.HATCHBACK,
                    ["Focus"] = TipoCarroceria.HATCHBACK,
                    ["Fusion"] = TipoCarroceria.SEDAN,
                    ["Taurus"] = TipoCarroceria.SEDAN,
                    ["Mondeo"] = TipoCarroceria.SEDAN,
                    ["Ka"] = TipoCarroceria.HATCHBACK,
                    ["Ikon"] = TipoCarroceria.SEDAN,
                    ["Crown Victoria"] = TipoCarroceria.SEDAN,
                    ["Bronco"] = TipoCarroceria.SUV,
                    ["Bronco Sport"] = TipoCarroceria.SUV,
                    ["EcoSport"] = TipoCarroceria.SUV,
                    ["Edge"] = TipoCarroceria.SUV,
                    ["Escape"] = TipoCarroceria.SUV,
                    ["Everest"] = TipoCarroceria.SUV,
                    ["Explorer"] = TipoCarroceria.SUV,
                    ["Expedition"] = TipoCarroceria.SUV,
                    ["Kuga"] = TipoCarroceria.SUV,
                    ["Puma"] = TipoCarroceria.SUV,
                    ["Territory"] = TipoCarroceria.SUV,
                    ["Maverick"] = TipoCarroceria.PICKUP,
                    ["Ranger"] = TipoCarroceria.PICKUP,
                    ["F-150"] = TipoCarroceria.PICKUP,
                    ["F-150 Lightning"] = TipoCarroceria.PICKUP,
                    ["F-250"] = TipoCarroceria.PICKUP,
                    ["F-350"] = TipoCarroceria.PICKUP,
                    ["F-450"] = TipoCarroceria.PICKUP,
                    ["Courier"] = TipoCarroceria.PICKUP,
                    ["Mustang"] = TipoCarroceria.AUTOSTURBO,
                    ["Mustang Mach-E"] = TipoCarroceria.SUV,
                    ["Ford GT"] = TipoCarroceria.AUTOSTURBO,
                    ["Transit"] = TipoCarroceria.MINIVAN,
                    ["Transit Connect"] = TipoCarroceria.MINIVAN,
                    ["Tourneo"] = TipoCarroceria.MINIVAN,
                },
                // ─── Nissan ───────────────────────────────────────────────────
                ["Nissan"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    // Sedanes
                    ["Altima"] = TipoCarroceria.SEDAN,
                    ["Sentra"] = TipoCarroceria.SEDAN,
                    ["Versa"] = TipoCarroceria.SEDAN,
                    ["Tiida"] = TipoCarroceria.SEDAN,
                    ["Maxima"] = TipoCarroceria.SEDAN,
                    ["Sylphy"] = TipoCarroceria.SEDAN,
                    ["Almera"] = TipoCarroceria.SEDAN,
                    ["Sunny"] = TipoCarroceria.SEDAN,
                    // Hatchbacks
                    ["March"] = TipoCarroceria.HATCHBACK,
                    ["Versa Note"] = TipoCarroceria.HATCHBACK,
                    ["Micra"] = TipoCarroceria.HATCHBACK,
                    ["Note"] = TipoCarroceria.HATCHBACK,
                    ["Leaf"] = TipoCarroceria.HATCHBACK,
                    // SUVs
                    ["Juke"] = TipoCarroceria.SUV,
                    ["Kicks"] = TipoCarroceria.SUV,
                    ["Kicks Play"] = TipoCarroceria.SUV,
                    ["Qashqai"] = TipoCarroceria.SUV,
                    ["Rogue"] = TipoCarroceria.SUV,
                    ["Rogue Sport"] = TipoCarroceria.SUV,
                    ["X-Trail"] = TipoCarroceria.SUV,
                    ["Murano"] = TipoCarroceria.SUV,
                    ["Pathfinder"] = TipoCarroceria.SUV,
                    ["Armada"] = TipoCarroceria.SUV,
                    ["Terrano"] = TipoCarroceria.SUV,
                    ["Ariya"] = TipoCarroceria.SUV,
                    ["Ariya e-4ORCE"] = TipoCarroceria.SUV,
                    ["Patrol"] = TipoCarroceria.SUV,
                    ["Xterra"] = TipoCarroceria.SUV,
                    // Pickups
                    ["Frontier"] = TipoCarroceria.PICKUP,
                    ["NP300"] = TipoCarroceria.PICKUP,
                    ["Navara"] = TipoCarroceria.PICKUP,
                    ["Titan"] = TipoCarroceria.PICKUP,
                    // Deportivos
                    ["370Z"] = TipoCarroceria.AUTOSTURBO,
                    ["400Z"] = TipoCarroceria.AUTOSTURBO,
                    ["GT-R"] = TipoCarroceria.AUTOSTURBO,
                    // Minivans
                    ["Urvan"] = TipoCarroceria.MINIVAN,
                    ["NV200"] = TipoCarroceria.MINIVAN,
                    ["NV350"] = TipoCarroceria.MINIVAN,
                },
                // ─── Toyota ───────────────────────────────────────────────────
                ["Toyota"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    // Sedanes
                    ["Camry"] = TipoCarroceria.SEDAN,
                    ["Corolla"] = TipoCarroceria.SEDAN,
                    ["Corolla Cross"] = TipoCarroceria.SUV,
                    ["Prius"] = TipoCarroceria.SEDAN,
                    ["Prius C"] = TipoCarroceria.HATCHBACK,
                    ["Yaris Sedan"] = TipoCarroceria.SEDAN,
                    // Hatchbacks
                    ["Yaris Hatchback"] = TipoCarroceria.HATCHBACK,
                    ["Yaris GR"] = TipoCarroceria.HATCHBACK,
                    ["GR86"] = TipoCarroceria.AUTOSTURBO,
                    // CUVs
                    ["4Runner"] = TipoCarroceria.SUV,
                    ["bZ4X"] = TipoCarroceria.SUV,
                    ["C-HR"] = TipoCarroceria.SUV,
                    ["Crown"] = TipoCarroceria.SUV,
                    ["Fortuner"] = TipoCarroceria.SUV,
                    ["Land Cruiser"] = TipoCarroceria.SUV,
                    ["Land Cruiser Prado"] = TipoCarroceria.SUV,
                    ["RAV4"] = TipoCarroceria.SUV,
                    ["Sequoia"] = TipoCarroceria.SUV,
                    ["Venza"] = TipoCarroceria.SUV,
                    ["Raize"] = TipoCarroceria.SUV,
                    // Pickups
                    ["Hilux"] = TipoCarroceria.PICKUP,
                    ["Tacoma"] = TipoCarroceria.PICKUP,
                    ["Tundra"] = TipoCarroceria.PICKUP,
                    // Minivans
                    ["Hiace"] = TipoCarroceria.MINIVAN,
                    ["Sienna"] = TipoCarroceria.MINIVAN,
                },
                // ─── Lexus ────────────────────────────────────────────────────
                ["Lexus"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["UX"] = TipoCarroceria.SUV,
                    ["IS"] = TipoCarroceria.SEDAN,
                    ["ES"] = TipoCarroceria.SEDAN,
                    ["NX"] = TipoCarroceria.SUV,
                    ["RX"] = TipoCarroceria.SUV,
                    ["TX"] = TipoCarroceria.SUV,
                    ["GX"] = TipoCarroceria.SUV,
                    ["LX"] = TipoCarroceria.SUV,
                    ["LC"] = TipoCarroceria.AUTOSTURBO,
                    ["LS"] = TipoCarroceria.SEDAN,
                    ["RZ"] = TipoCarroceria.SUV,
                },
                // ─── Volkswagen ───────────────────────────────────────────────
                ["Volkswagen"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    // Hatchbacks
                    ["Beetle"] = TipoCarroceria.HATCHBACK,
                    ["Crossfox"] = TipoCarroceria.HATCHBACK,
                    ["Fox"] = TipoCarroceria.HATCHBACK,
                    ["Gol"] = TipoCarroceria.HATCHBACK,
                    ["Golf"] = TipoCarroceria.HATCHBACK,
                    ["Golf GTI"] = TipoCarroceria.HATCHBACK,
                    ["Golf R"] = TipoCarroceria.HATCHBACK,
                    ["Lupo"] = TipoCarroceria.HATCHBACK,
                    ["Polo"] = TipoCarroceria.HATCHBACK,
                    ["Up!"] = TipoCarroceria.HATCHBACK,
                    // Sedanes
                    ["Bora"] = TipoCarroceria.SEDAN,
                    ["Jetta"] = TipoCarroceria.SEDAN,
                    ["Passat"] = TipoCarroceria.SEDAN,
                    ["Phaeton"] = TipoCarroceria.SEDAN,
                    ["Vento"] = TipoCarroceria.SEDAN,
                    ["Virtus"] = TipoCarroceria.SEDAN,
                    // CUVs
                    ["Taigun"] = TipoCarroceria.SUV,
                    ["Taos"] = TipoCarroceria.SUV,
                    ["T-Cross"] = TipoCarroceria.SUV,
                    ["T-Roc"] = TipoCarroceria.SUV,
                    ["Tiguan"] = TipoCarroceria.SUV,
                    ["Tiguan Allspace"] = TipoCarroceria.SUV,
                    ["Teramont"] = TipoCarroceria.SUV,
                    ["Touareg"] = TipoCarroceria.SUV,
                    // Pickups
                    ["Amarok"] = TipoCarroceria.PICKUP,
                    ["Saveiro"] = TipoCarroceria.PICKUP,
                    // Minivans
                    ["Caravelle"] = TipoCarroceria.MINIVAN,
                    ["Multivan"] = TipoCarroceria.MINIVAN,
                    ["Routan"] = TipoCarroceria.MINIVAN,
                    ["Sharan"] = TipoCarroceria.MINIVAN,
                    ["Touran"] = TipoCarroceria.MINIVAN,
                    ["Transporter"] = TipoCarroceria.MINIVAN,
                    // Eléctricos (CUV/Hatchback)
                    ["ID.3"] = TipoCarroceria.HATCHBACK,
                    ["ID.4"] = TipoCarroceria.SUV,
                    ["ID.5"] = TipoCarroceria.SUV,
                    ["ID.6"] = TipoCarroceria.SUV,
                    ["ID.7"] = TipoCarroceria.SEDAN,
                    ["ID. Buzz"] = TipoCarroceria.MINIVAN,
                    // Deportivos
                    ["Arteon"] = TipoCarroceria.AUTOSTURBO,
                    ["CC"] = TipoCarroceria.AUTOSTURBO,
                    ["Corrado"] = TipoCarroceria.AUTOSTURBO,
                    ["Scirocco"] = TipoCarroceria.AUTOSTURBO,
                },
                // ─── Honda ────────────────────────────────────────────────────
                ["Honda"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Accord"] = TipoCarroceria.SEDAN,
                    ["BR-V"] = TipoCarroceria.SUV,
                    ["City"] = TipoCarroceria.SEDAN,
                    ["Civic"] = TipoCarroceria.SEDAN,
                    ["CR-V"] = TipoCarroceria.SUV,
                    ["Fit"] = TipoCarroceria.HATCHBACK,
                    ["HR-V"] = TipoCarroceria.SUV,
                    ["Insight"] = TipoCarroceria.SEDAN,
                    ["WR-V"] = TipoCarroceria.SUV,
                    ["CR-Z"] = TipoCarroceria.HATCHBACK,
                    ["Crosstour"] = TipoCarroceria.SUV,
                    ["Element"] = TipoCarroceria.SUV,
                    ["Odyssey"] = TipoCarroceria.MINIVAN,
                    ["Pilot"] = TipoCarroceria.SUV,
                    ["Ridgeline"] = TipoCarroceria.PICKUP,
                },
                // ─── Hyundai ──────────────────────────────────────────────────
                ["Hyundai"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Accent"] = TipoCarroceria.SEDAN,
                    ["Creta"] = TipoCarroceria.SUV,
                    ["Elantra"] = TipoCarroceria.SEDAN,
                    ["Ioniq"] = TipoCarroceria.HATCHBACK,
                    ["Kona"] = TipoCarroceria.SUV,
                    ["Santa Fe"] = TipoCarroceria.SUV,
                    ["Sonata"] = TipoCarroceria.SEDAN,
                    ["Tucson"] = TipoCarroceria.SUV,
                    ["Venue"] = TipoCarroceria.SUV,
                },
                // ─── Kia ──────────────────────────────────────────────────────
                ["Kia"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Carnival"] = TipoCarroceria.MINIVAN,
                    ["EV6"] = TipoCarroceria.SUV,
                    ["K3"] = TipoCarroceria.SEDAN,
                    ["K4"] = TipoCarroceria.SEDAN,
                    ["K5"] = TipoCarroceria.SEDAN,
                    ["Niro"] = TipoCarroceria.SUV,
                    ["Rio"] = TipoCarroceria.HATCHBACK,
                    ["Seltos"] = TipoCarroceria.SUV,
                    ["Sorento"] = TipoCarroceria.SUV,
                    ["Soul"] = TipoCarroceria.HATCHBACK,
                    ["Sportage"] = TipoCarroceria.SUV,
                    ["Stinger"] = TipoCarroceria.AUTOSTURBO,
                },
                // ─── Mazda ────────────────────────────────────────────────────
                ["Mazda"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["CX-3"] = TipoCarroceria.SUV,
                    ["CX-30"] = TipoCarroceria.SUV,
                    ["CX-5"] = TipoCarroceria.SUV,
                    ["CX-7"] = TipoCarroceria.SUV,
                    ["CX-9"] = TipoCarroceria.SUV,
                    ["CX-50"] = TipoCarroceria.SUV,
                    ["CX-60"] = TipoCarroceria.SUV,
                    ["CX-70"] = TipoCarroceria.SUV,
                    ["CX-90"] = TipoCarroceria.SUV,
                    ["Mazda2"] = TipoCarroceria.HATCHBACK,
                    ["Mazda3"] = TipoCarroceria.SEDAN,
                    ["Mazda5"] = TipoCarroceria.MINIVAN,
                    ["Mazda6"] = TipoCarroceria.SEDAN,
                    ["MX-5"] = TipoCarroceria.AUTOSTURBO,
                    ["MX-30"] = TipoCarroceria.SUV,
                    ["BT-50"] = TipoCarroceria.PICKUP,
                },
                // ─── Dodge ────────────────────────────────────────────────────
                ["Dodge"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Attitude"] = TipoCarroceria.SEDAN,
                    ["Challenger"] = TipoCarroceria.AUTOSTURBO,
                    ["Charger"] = TipoCarroceria.AUTOSTURBO,
                    ["Durango"] = TipoCarroceria.SUV,
                    ["Journey"] = TipoCarroceria.SUV,
                    ["Neon"] = TipoCarroceria.SEDAN,
                    ["Vision"] = TipoCarroceria.SEDAN,
                    ["Grand Caravan"] = TipoCarroceria.MINIVAN,
                    ["Dakota"] = TipoCarroceria.PICKUP,
                    ["Viper"] = TipoCarroceria.AUTOSTURBO,
                    ["Nitro"] = TipoCarroceria.SUV,
                    ["Caliber"] = TipoCarroceria.HATCHBACK,
                    ["Avenger"] = TipoCarroceria.SEDAN,
                    ["i10"] = TipoCarroceria.HATCHBACK,
                },
                // ─── Jeep ─────────────────────────────────────────────────────
                ["Jeep"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Cherokee"] = TipoCarroceria.SUV,
                    ["Compass"] = TipoCarroceria.SUV,
                    ["Gladiator"] = TipoCarroceria.PICKUP,
                    ["Grand Cherokee"] = TipoCarroceria.SUV,
                    ["Renegade"] = TipoCarroceria.SUV,
                    ["Wrangler"] = TipoCarroceria.SUV,
                },
                // ─── RAM ──────────────────────────────────────────────────────
                ["RAM"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["700"] = TipoCarroceria.PICKUP,
                    ["1200"] = TipoCarroceria.PICKUP,
                    ["1500"] = TipoCarroceria.PICKUP,
                    ["2500"] = TipoCarroceria.PICKUP,
                    ["ProMaster"] = TipoCarroceria.MINIVAN,
                    ["3500"] = TipoCarroceria.PICKUP,
                },
                // ─── Audi ─────────────────────────────────────────────────────
                ["Audi"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["A1"] = TipoCarroceria.HATCHBACK,
                    ["A3"] = TipoCarroceria.SEDAN,
                    ["A4"] = TipoCarroceria.SEDAN,
                    ["A5"] = TipoCarroceria.AUTOSTURBO,
                    ["A6"] = TipoCarroceria.SEDAN,
                    ["Q2"] = TipoCarroceria.SUV,
                    ["Q3"] = TipoCarroceria.SUV,
                    ["Q5"] = TipoCarroceria.SUV,
                    ["Q7"] = TipoCarroceria.SUV,
                    ["Q8"] = TipoCarroceria.SUV,
                    ["TT"] = TipoCarroceria.AUTOSTURBO,
                },
                // ─── BMW ──────────────────────────────────────────────────────
                ["BMW"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["116i"] = TipoCarroceria.HATCHBACK,
                    ["118i"] = TipoCarroceria.HATCHBACK,
                    ["218i"] = TipoCarroceria.SEDAN,
                    ["320i"] = TipoCarroceria.SEDAN,
                    ["330i"] = TipoCarroceria.SEDAN,
                    ["420i"] = TipoCarroceria.AUTOSTURBO,
                    ["520i"] = TipoCarroceria.SEDAN,
                    ["530i"] = TipoCarroceria.SEDAN,
                    ["X1"] = TipoCarroceria.SUV,
                    ["X2"] = TipoCarroceria.SUV,
                    ["X3"] = TipoCarroceria.SUV,
                    ["X4"] = TipoCarroceria.SUV,
                    ["X5"] = TipoCarroceria.SUV,
                    ["X6"] = TipoCarroceria.SUV,
                },
                // ─── Mercedes-Benz ────────────────────────────────────────────
                ["Mercedes-Benz"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["A 200"] = TipoCarroceria.HATCHBACK,
                    ["B 200"] = TipoCarroceria.HATCHBACK,
                    ["C 180"] = TipoCarroceria.SEDAN,
                    ["C 200"] = TipoCarroceria.SEDAN,
                    ["CLA"] = TipoCarroceria.SEDAN,
                    ["E 200"] = TipoCarroceria.SEDAN,
                    ["GLA"] = TipoCarroceria.SUV,
                    ["GLB"] = TipoCarroceria.SUV,
                    ["GLC"] = TipoCarroceria.SUV,
                    ["GLE"] = TipoCarroceria.SUV,
                    ["GLS"] = TipoCarroceria.SUV,
                },
                // ─── Peugeot ──────────────────────────────────────────────────
                ["Peugeot"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["208"] = TipoCarroceria.HATCHBACK,
                    ["2008"] = TipoCarroceria.SUV,
                    ["3008"] = TipoCarroceria.SUV,
                    ["308"] = TipoCarroceria.HATCHBACK,
                    ["408"] = TipoCarroceria.SEDAN,
                    ["5008"] = TipoCarroceria.SUV,
                },
                // ─── Renault ──────────────────────────────────────────────────
                ["Renault"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Captur"] = TipoCarroceria.SUV,
                    ["Duster"] = TipoCarroceria.SUV,
                    ["Kangoo"] = TipoCarroceria.MINIVAN,
                    ["Koleos"] = TipoCarroceria.SUV,
                    ["Logan"] = TipoCarroceria.SEDAN,
                    ["Oroch"] = TipoCarroceria.PICKUP,
                    ["Sandero"] = TipoCarroceria.HATCHBACK,
                    ["Stepway"] = TipoCarroceria.SUV,
                },
                // ─── SEAT ─────────────────────────────────────────────────────
                ["SEAT"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Arona"] = TipoCarroceria.SUV,
                    ["Ateca"] = TipoCarroceria.SUV,
                    ["Ibiza"] = TipoCarroceria.HATCHBACK,
                    ["Leon"] = TipoCarroceria.HATCHBACK,
                    ["Leon Sportstourer"] = TipoCarroceria.HATCHBACK,
                    ["Tarraco"] = TipoCarroceria.SUV,
                    ["Toledo"] = TipoCarroceria.SEDAN,
                    ["Altea"] = TipoCarroceria.HATCHBACK,
                    ["Altea XL"] = TipoCarroceria.HATCHBACK,
                    ["Exeo"] = TipoCarroceria.SEDAN,
                    ["Mii"] = TipoCarroceria.HATCHBACK,
                    ["Cordoba"] = TipoCarroceria.SEDAN,
                    ["Freetrack"] = TipoCarroceria.SUV,
                },
                // ─── CUPRA ────────────────────────────────────────────────────
                ["CUPRA"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Formentor"] = TipoCarroceria.SUV,
                    ["Leon"] = TipoCarroceria.HATCHBACK,
                    ["Ateca"] = TipoCarroceria.SUV,
                    ["Born"] = TipoCarroceria.HATCHBACK,
                },
                // ─── Subaru ───────────────────────────────────────────────────
                ["Subaru"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["BRZ"] = TipoCarroceria.AUTOSTURBO,
                    ["Crosstrek"] = TipoCarroceria.SUV,
                    ["Forester"] = TipoCarroceria.SUV,
                    ["Impreza"] = TipoCarroceria.HATCHBACK,
                    ["Legacy"] = TipoCarroceria.SEDAN,
                    ["Outback"] = TipoCarroceria.SUV,
                    ["WRX"] = TipoCarroceria.SEDAN,
                },
                // ─── Mitsubishi ───────────────────────────────────────────────
                ["Mitsubishi"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Eclipse Cross"] = TipoCarroceria.SUV,
                    ["L200"] = TipoCarroceria.PICKUP,
                    ["Mirage"] = TipoCarroceria.HATCHBACK,
                    ["Montero"] = TipoCarroceria.SUV,
                    ["Outlander"] = TipoCarroceria.SUV,
                },
                // ─── Fiat ─────────────────────────────────────────────────────
                ["Fiat"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Argo"] = TipoCarroceria.HATCHBACK,
                    ["Cronos"] = TipoCarroceria.SEDAN,
                    ["Mobi"] = TipoCarroceria.HATCHBACK,
                    ["Pulse"] = TipoCarroceria.SUV,
                    ["Toro"] = TipoCarroceria.PICKUP,
                },
                // ─── Suzuki ───────────────────────────────────────────────────
                ["Suzuki"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Baleno"] = TipoCarroceria.SEDAN,
                    ["Ignis"] = TipoCarroceria.HATCHBACK,
                    ["Jimny"] = TipoCarroceria.SUV,
                    ["Swift"] = TipoCarroceria.HATCHBACK,
                    ["Vitara"] = TipoCarroceria.SUV,
                },
                // ─── Volvo ────────────────────────────────────────────────────
                ["Volvo"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["S60"] = TipoCarroceria.SEDAN,
                    ["S90"] = TipoCarroceria.SEDAN,
                    ["V60"] = TipoCarroceria.SEDAN,
                    ["XC40"] = TipoCarroceria.SUV,
                    ["XC60"] = TipoCarroceria.SUV,
                    ["XC90"] = TipoCarroceria.SUV,
                },
                // ─── GMC ──────────────────────────────────────────────────────
                ["GMC"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Acadia"] = TipoCarroceria.SUV,
                    ["Canyon"] = TipoCarroceria.PICKUP,
                    ["Sierra"] = TipoCarroceria.PICKUP,
                    ["Terrain"] = TipoCarroceria.SUV,
                    ["Yukon"] = TipoCarroceria.SUV,
                },
                // ─── Buick ────────────────────────────────────────────────────
                ["Buick"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Enclave"] = TipoCarroceria.SUV,
                    ["Encore"] = TipoCarroceria.SUV,
                    ["Envision"] = TipoCarroceria.SUV,
                },
                // ─── Chrysler ─────────────────────────────────────────────────
                ["Chrysler"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["300"] = TipoCarroceria.SEDAN,
                    ["Pacifica"] = TipoCarroceria.MINIVAN,
                },
                // ─── Mini ─────────────────────────────────────────────────────
                ["Mini"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Clubman"] = TipoCarroceria.HATCHBACK,
                    ["Cooper"] = TipoCarroceria.HATCHBACK,
                    ["Countryman"] = TipoCarroceria.SUV,
                    ["Paceman"] = TipoCarroceria.SUV,
                },
                // ─── Porsche ──────────────────────────────────────────────────
                ["Porsche"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["718"] = TipoCarroceria.AUTOSTURBO,
                    ["911"] = TipoCarroceria.AUTOSTURBO,
                    ["Cayenne"] = TipoCarroceria.SUV,
                    ["Macan"] = TipoCarroceria.SUV,
                    ["Panamera"] = TipoCarroceria.SEDAN,
                    ["Taycan"] = TipoCarroceria.SEDAN,
                },
                // ─── Land Rover ───────────────────────────────────────────────
                ["Land Rover"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Defender"] = TipoCarroceria.SUV,
                    ["Discovery"] = TipoCarroceria.SUV,
                    ["Discovery Sport"] = TipoCarroceria.SUV,
                    ["Freelander"] = TipoCarroceria.SUV,
                    ["Range Rover"] = TipoCarroceria.SUV,
                    ["Range Rover Evoque"] = TipoCarroceria.SUV,
                    ["Range Rover Sport"] = TipoCarroceria.SUV,
                },
                // ─── Tesla ────────────────────────────────────────────────────
                ["Tesla"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Model 3"] = TipoCarroceria.SEDAN,
                    ["Model S"] = TipoCarroceria.SEDAN,
                    ["Model X"] = TipoCarroceria.SUV,
                    ["Model Y"] = TipoCarroceria.SUV,
                },
                // ─── MG ───────────────────────────────────────────────────────
                ["MG"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["MG3"] = TipoCarroceria.HATCHBACK,
                    ["MG5"] = TipoCarroceria.SEDAN,
                    ["MG GT"] = TipoCarroceria.SEDAN,
                    ["ZS"] = TipoCarroceria.SUV,
                    ["ZS EV"] = TipoCarroceria.SUV,
                    ["HS"] = TipoCarroceria.SUV,
                    ["RX5"] = TipoCarroceria.SUV,
                    ["Marvel R"] = TipoCarroceria.SUV,
                },
                // ─── BYD ──────────────────────────────────────────────────────
                ["BYD"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Dolphin"] = TipoCarroceria.HATCHBACK,
                    ["Dolphin Mini"] = TipoCarroceria.HATCHBACK,
                    ["Seal"] = TipoCarroceria.SEDAN,
                    ["Song Plus"] = TipoCarroceria.SUV,
                    ["Yuan Plus"] = TipoCarroceria.SUV,
                    ["Tang"] = TipoCarroceria.SUV,
                    ["Han"] = TipoCarroceria.SEDAN,
                },
                // ─── Chirey ───────────────────────────────────────────────────
                ["Chirey"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Tiggo 2 Pro"] = TipoCarroceria.SUV,
                    ["Tiggo 4 Pro"] = TipoCarroceria.SUV,
                    ["Tiggo 7 Pro"] = TipoCarroceria.SUV,
                    ["Tiggo 8 Pro"] = TipoCarroceria.SUV,
                },
                // ─── Omoda ────────────────────────────────────────────────────
                ["Omoda"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["O5"] = TipoCarroceria.SUV,
                    ["C5"] = TipoCarroceria.SUV,
                },
                // ─── JAC ──────────────────────────────────────────────────────
                ["JAC"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["J7"] = TipoCarroceria.SEDAN,
                    ["J4"] = TipoCarroceria.SEDAN,
                    ["Sei2"] = TipoCarroceria.SUV,
                    ["Sei3"] = TipoCarroceria.SUV,
                    ["Sei4 Pro"] = TipoCarroceria.SUV,
                    ["Sei7 Pro"] = TipoCarroceria.SUV,
                    ["E10X"] = TipoCarroceria.MINIVAN,
                    ["E J7"] = TipoCarroceria.SEDAN,
                    ["Frison T6"] = TipoCarroceria.PICKUP,
                    ["Frison T8"] = TipoCarroceria.PICKUP,
                },
                // ─── GWM ──────────────────────────────────────────────────────
                ["GWM"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Haval H6"] = TipoCarroceria.SUV,
                    ["Haval Jolion"] = TipoCarroceria.SUV,
                    ["Tank 300"] = TipoCarroceria.SUV,
                    ["Ora 03"] = TipoCarroceria.HATCHBACK,
                },
                // ─── BAIC ─────────────────────────────────────────────────────
                ["BAIC"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["X35"] = TipoCarroceria.SUV,
                    ["X55"] = TipoCarroceria.SUV,
                    ["BJ40"] = TipoCarroceria.SUV,
                    ["EU5"] = TipoCarroceria.SEDAN,
                },
                // ─── Changan ──────────────────────────────────────────────────
                ["Changan"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Alsvin"] = TipoCarroceria.SEDAN,
                    ["CS35 Plus"] = TipoCarroceria.SUV,
                    ["CS55 Plus"] = TipoCarroceria.SUV,
                    ["CS75 Pro"] = TipoCarroceria.SUV,
                    ["UNI-T"] = TipoCarroceria.SUV,
                    ["UNI-K"] = TipoCarroceria.SUV,
                },
                // ─── Jetour ───────────────────────────────────────────────────
                ["Jetour"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["X70"] = TipoCarroceria.SUV,
                    ["X70 Plus"] = TipoCarroceria.SUV,
                    ["Dashing"] = TipoCarroceria.SUV,
                    ["T2"] = TipoCarroceria.SUV,
                },
                // ─── Lincoln ──────────────────────────────────────────────────
                ["Lincoln"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Corsair"] = TipoCarroceria.SUV,
                    ["Nautilus"] = TipoCarroceria.SUV,
                    ["Aviator"] = TipoCarroceria.SUV,
                    ["Navigator"] = TipoCarroceria.SUV,
                },
                // ─── Smart ────────────────────────────────────────────────────
                ["Smart"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Fortwo"] = TipoCarroceria.HATCHBACK,
                    ["Forfour"] = TipoCarroceria.HATCHBACK,
                    ["#1"] = TipoCarroceria.SUV,
                    ["#6"] = TipoCarroceria.SUV,
                },
                // ─── Geely ────────────────────────────────────────────────────
                ["Geely"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Coolray"] = TipoCarroceria.SUV,
                    ["Emgrand"] = TipoCarroceria.SEDAN,
                    ["Cityray"] = TipoCarroceria.SUV,
                    ["Starray"] = TipoCarroceria.SUV,
                    ["EX2"] = TipoCarroceria.HATCHBACK,
                    ["EX5"] = TipoCarroceria.SUV,
                    ["Monjaro"] = TipoCarroceria.SUV,
                    ["Okavango"] = TipoCarroceria.SUV,
                    ["GX3 Pro"] = TipoCarroceria.SUV,
                },
                // ─── Acura ────────────────────────────────────────────────────
                ["Acura"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["ILX"] = TipoCarroceria.SEDAN,
                    ["TLX"] = TipoCarroceria.SEDAN,
                    ["RDX"] = TipoCarroceria.SUV,
                    ["MDX"] = TipoCarroceria.SUV,
                    ["ZDX"] = TipoCarroceria.SUV,
                },
                // ─── Infiniti ─────────────────────────────────────────────────
                ["Infiniti"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["QX50"] = TipoCarroceria.SUV,
                    ["QX55"] = TipoCarroceria.SUV,
                    ["QX60"] = TipoCarroceria.SUV,
                    ["QX80"] = TipoCarroceria.SUV,
                },
                // ─── Isuzu ────────────────────────────────────────────────────
                ["Isuzu"] = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["D-Max"] = TipoCarroceria.PICKUP,
                    ["MU-X"] = TipoCarroceria.SUV,
                },
            };


        // ══════════════════════════════════════════════════════════════════════
        //  MÉTODOS PÚBLICOS
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>Lista de todas las marcas ordenada alfabéticamente.</summary>
        public static List<string> ObtenerMarcas() =>
            _catalogo.Keys.OrderBy(m => m).ToList();

        /// <summary>Lista de modelos para una marca dada. Vacía si la marca no existe.</summary>
        public static List<string> ObtenerModelos(string marca)
        {
            if (string.IsNullOrWhiteSpace(marca)) return new();
            return _catalogo.TryGetValue(marca, out var modelos)
                ? modelos.Keys.OrderBy(m => m).ToList()
                : new();
        }

        /// <summary>Lista de modelos filtrados por tipo de carrocería para una marca dada.</summary>
        public static List<string> ObtenerModelosPorTipo(string marca, TipoCarroceria tipo)
        {
            if (string.IsNullOrWhiteSpace(marca)) return new();
            if (!_catalogo.TryGetValue(marca, out var modelos)) return new();
            if (!_tipos.TryGetValue(marca, out var tiposModelos)) return new();

            return modelos.Keys
                .Where(m => tiposModelos.TryGetValue(m, out var t) && t == tipo)
                .OrderBy(m => m)
                .ToList();
        }

        /// <summary>Lista de versiones para una marca + modelo. Vacía si no existe.</summary>
        public static List<string> ObtenerVersiones(string marca, string modelo)
        {
            if (string.IsNullOrWhiteSpace(marca) || string.IsNullOrWhiteSpace(modelo)) return new();
            if (!_catalogo.TryGetValue(marca, out var modelos)) return new();
            return modelos.TryGetValue(modelo, out var versiones) ? versiones : new();
        }

        /// <summary>Devuelve el tipo de carrocería de un modelo. Null si no está clasificado.</summary>
        public static TipoCarroceria? ObtenerTipo(string marca, string modelo)
        {
            if (string.IsNullOrWhiteSpace(marca) || string.IsNullOrWhiteSpace(modelo)) return null;
            if (!_tipos.TryGetValue(marca, out var tiposModelos)) return null;
            return tiposModelos.TryGetValue(modelo, out var tipo) ? tipo : null;
        }

        /// <summary>True si la marca existe en el catálogo.</summary>
        public static bool ExisteMarca(string marca) =>
            !string.IsNullOrWhiteSpace(marca) && _catalogo.ContainsKey(marca);

        /// <summary>True si el modelo existe para esa marca.</summary>
        public static bool ExisteModelo(string marca, string modelo) =>
            ExisteMarca(marca) && _catalogo[marca].ContainsKey(modelo);
    }
}