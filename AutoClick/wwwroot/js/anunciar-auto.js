// AnunciarMiAuto JavaScript functionality

// Datos de marcas y modelos (A-M)
const marcasModelos = {
    "Acura": ["Legend", "Integra", "Vigor", "NSX", "SLX", "3.2 TL", "TL", "RL", "RSX", "MDX", "TSX", "RDX", "ILX", "RLX"],
    "Aion": ["Aion Hyper GT", "Aion Hyper SSR", "Aion LX Plus", "Aion RT", "Aion S", "Aion S Max", "Aion S Plus", "Aion UT", "Aion V Plus", "Aion Y Plus", "Hyptec HT", "Hyptec SSR"],
    "Alfa Romeo": ["145", "146", "147", "155", "156", "159", "164", "166", "1900", "2000", "2600", "33", "33 Stradale", "4C", "6C", "75", "8C Competizione", "Alfetta", "Alfasud", "Arna", "Brera", "Disco Volante", "Giulia", "Giulietta", "GT", "GTV", "Junior", "MiTo", "Montreal", "Spider", "Sprint", "Stelvio", "SZ", "Tonale"],
    "Aston Martin": ["Vantage", "DB12", "Vanquish", "DBX", "Valhalla", "Valkyrie", "Valour", "Valiant", "AMR25", "DB1", "DB2", "DB2/4", "DB4", "DB5", "DB6", "DBS", "V8", "Virage", "DB7", "DB9", "V8 Vantage", "DBS V12", "Rapide", "V12 Vantage", "DB11", "DBS Superleggera", "DBX707", "DB12 Volante", "DB12 Goldfinger", "V12 Speedster", "Vantage 007 Edition", "DBS 770 Ultimate", "V12 Vantage Roadster", "DBX S"],
    "Audi": ["A3", "S3", "RS 3", "A4", "S4", "RS 4", "A5", "S5", "RS 5", "A6", "S6", "RS 6", "A7", "S7", "RS 7", "A8", "S8", "RS 8", "A6 e-tron", "S6 e-tron", "RS e-tron GT", "Q3", "Q3 Sportback", "Q4 e-tron", "Q4 Sportback e-tron", "Q5", "Q5 Sportback", "SQ5", "SQ5 Sportback", "Q6 e-tron", "SQ6 e-tron", "Q6 Sportback e-tron", "SQ6 Sportback e-tron", "Q7", "SQ7", "Q8", "SQ8", "Q8 e-tron", "SQ8 e-tron", "RS Q3", "RS Q5", "RS Q7", "RS Q8", "TT", "TT RS", "R8", "R8 V10"],
    "Austin": ["A30", "A35", "A40", "A50", "A55", "A60", "A70", "A75", "A80", "A90", "A95", "A99", "Healey 100", "Healey 3000", "Mini", "Princess", "Metro", "Allegro", "Maestro", "Montego", "Maxi", "1100", "1800", "FX4 (Taxi)"],
    "BAIC": ["BJ20", "BJ40", "BJ60", "BJ80", "EU5", "EU7", "EU260", "EU400", "X25", "X35", "X55", "X65", "Senova D20", "Senova D50", "Senova D60", "Senova X25", "Senova X35", "Senova X55", "Senova X65", "Weiwang M20", "Weiwang M30", "Weiwang M50F", "Weiwang S50", "Weiwang S50F", "Z40", "Z50", "Z30"],
    "BAW": ["BJ212", "BJ2022", "BJ2032", "BJ2030", "BJ80", "Luba", "Luba V3", "Luba V5", "Land King", "Tonik", "Raptor"],
    "Bentley": ["Bentayga", "Continental GT", "Continental GT Convertible", "Flying Spur", "Mulsanne", "Arnage", "Azure", "Brooklands", "Continental R", "Continental T", "Continental SC", "Turbo R", "Turbo S", "Bentayga Speed", "Bacalar"],
    "Bluebird": ["Bluebird", "Bluebird Sylphy", "Bluebird Maxima", "Bluebird 910", "Bluebird 810", "Bluebird 910 Coupe", "Bluebird U12", "Bluebird 410", "Bluebird SSS", "Bluebird GX"],
    "BMW": ["Serie 1", "Serie 2", "Serie 3", "Serie 4", "Serie 5", "Serie 6", "Serie 7", "Serie 8", "X1", "X2", "X3", "X4", "X5", "X6", "X7", "Z3", "Z4", "Z8", "i3", "i4", "i7", "iX", "iX3", "iX5", "iX6", "iX7", "M2", "M3", "M4", "M5", "M6", "M8", "XM", "X3 M", "X4 M", "X5 M", "X6 M", "X7 M"],
    "Brilliance": ["V3", "V5", "V6", "H230", "H320", "H330", "H530", "H530F", "H330F", "H220", "BS4", "BS6", "BS2", "FRV", "FSV", "M2", "M3", "M1", "V7", "Jinbei S30", "Jinbei S50"],
    "Buick": ["Enclave", "Encore", "Encore GX", "Encore Plus", "Envision", "LaCrosse", "Regal", "Regal GS", "Verano", "Rainier", "Rendezvous", "LeSabre", "Century", "Electra", "Park Avenue", "Skylark", "Roadmaster", "Wildcat", "Riviera"],
    "BYD": ["Tang", "Tang DM", "Tang EV", "Han", "Han EV", "Han DM", "Qin", "Qin Pro", "Qin Plus", "Qin EV", "Song", "Song Pro", "Song Max", "Song Plus", "Song EV", "Yuan", "Yuan Plus", "Yuan EV", "Seal", "Dolphin", "e2", "e3", "e5", "Atto 3", "Atto 4", "D1", "D3", "e6", "F3", "F3 DM", "F3 EV", "S2", "S3", "S5"],
    "Cadillac": ["Escalade", "Escalade ESV", "CT4", "CT5", "CT6", "XT4", "XT5", "XT6", "Lyriq", "Seville", "Eldorado", "DeVille", "Fleetwood", "Brougham", "Allanté", "Catera", "SRX", "STS", "DTS", "Cimarron", "XLR", "ATS"],
    "Chana": ["Star", "Star 2", "Star 3", "Star 7", "Benni", "Benni Mini", "Benni EV", "CM7", "CM8", "CS15", "CS35", "CS35 Plus", "CS55", "CS55 Plus", "CS75", "CS75 Plus", "CS95", "CX70", "CX70T", "CX30", "CX30 EV", "V3", "V5", "V7", "V8"],
    "Changan": ["Alsvin", "Alsvin V3", "Alsvin V5", "Alsvin V7", "Eado", "Eado XT", "Eado Plus", "Eado EV", "Raeton", "Raeton CC", "CS15", "CS15 Plus", "CS35", "CS35 Plus", "CS55", "CS55 Plus", "CS75", "CS75 Plus", "CS85", "CS95", "UNI-T", "UNI-K", "UNI-V", "CS10", "CS85 Coupe", "Benni", "Benni EV", "Oshan X7", "Oshan X5", "Oshan Z6"],
    "Chery": ["Arrizo 3", "Arrizo 5", "Arrizo 6", "Arrizo 5e", "Arrizo GX", "Arrizo EX", "Arrizo 8", "Tiggo 2", "Tiggo 3", "Tiggo 3x", "Tiggo 4", "Tiggo 5", "Tiggo 5x", "Tiggo 7", "Tiggo 7 Pro", "Tiggo 7 Pro MAX", "Tiggo 8", "Tiggo 8 Pro", "Tiggo 8 Pro MAX", "Tiggo 9", "QQ", "QQ3", "QQ6", "Fulwin", "A1", "A3", "A5", "Eastar", "V5", "Cowin X3", "Cowin X5", "Cowin X7"],
    "Chevrolet": ["Aveo", "Beat", "Spark", "Sonic", "Malibu", "Cruze", "Camaro", "Corvette", "Trax", "Tracker", "Equinox", "Blazer", "Traverse", "Tahoe", "Suburban", "Colorado", "Silverado", "Silverado HD", "S10", "Trailblazer", "Bolt EV", "Bolt EUV", "Onix", "Onix Plus", "Sail", "Captiva", "Orlando", "Celta", "Spin", "Montana", "Astro", "Impala", "Lumina", "Avalanche", "SSR", "HHR", "Tahoe Hybrid"],
    "Chrysler": ["300", "Pacifica", "Voyager", "Aspen", "Sebring", "PT Cruiser", "Town & Country", "Neon", "Concorde", "Cirrus", "LHS", "Crossfire", "200", "100", "Saratoga", "Imperial", "Airflow", "Windsor", "Newport", "Royal", "Valiant", "Airflow C-8", "Royal Windsor", "Fifth Avenue"],
    "Citroen": ["C1", "C2", "C3", "C3 Aircross", "C3 Picasso", "C4", "C4 Cactus", "C4 SpaceTourer", "C4 Picasso", "C5", "C5 Aircross", "C6", "C-Elysée", "Berlingo", "Nemo", "Jumpy", "Spacetourer", "Jumper", "Saxo", "Xsara", "Xsara Picasso", "ZX", "DS3", "DS4", "DS5"],
    "CMC": ["Veryca", "Veryca II", "Zinger", "CMC Adventure", "CMC P350 Hybrid", "CMC P350 Electric", "CMC Leadcar", "CMC Luxgen V7", "CMC Luxgen U7", "CMC Luxgen S3", "CMC Luxgen S5", "CMC Luxgen M7"],
    "Dacia": ["Sandero", "Sandero Stepway", "Logan", "Logan MCV", "Dokker", "Dokker Van", "Duster", "Lodgy", "Jogger", "Spring", "Bigster"],
    "Daewoo": ["Sandero", "Sandero Stepway", "Logan", "Logan MCV", "Dokker", "Dokker Van", "Duster", "Lodgy", "Jogger", "Spring", "Bigster"],
    "Daihatsu": ["Ayla", "Terios", "Sirion", "Rocky", "Taft", "Tanto", "Mira", "Mira e:S", "Mira Gino", "Move", "Move Conte", "Move Custom", "Copen", "Charade", "Feroza", "Hijet", "Materia", "Sigra", "Luxio", "YRV", "Cuore", "Boon"],
    "Datsun": ["Go", "Go+", "Go Cross", "Redi-GO", "mi-DO", "on-DO", "mi-DO Cross", "240Z", "280Z", "510", "620", "720", "810", "1200", "Bluebird", "Stanza", "Cherry", "Go-Premier", "GO T", "Go Live"],
    "Dodge": ["Charger", "Challenger", "Durango", "Journey", "Grand Caravan", "Ram 1500", "Ram 2500", "Ram 3500", "Dakota", "Viper", "Neon", "Intrepid", "Stratus", "Avenger", "Caliber", "Nitro", "Magnum", "Dart", "Omni", "Polara", "Shadow", "Stealth", "Spirit", "Omni GLH", "Rampage"],
    "Dongfeng": ["Fengguang 330", "Fengguang 360", "Fengguang 370", "Fengguang 580", "Fengguang 580 Pro", "Fengguang 580 Max", "Fengon 500", "Fengon 580", "Fengon 580 Pro", "Fengon 580 Max", "Fengon ix5", "Fengon ix7", "Fengshen AX7", "Fengshen A60", "Fengshen L60", "Fengshen E70", "Rich 6", "Rich 7", "Rich 8", "Mini EV"],
    "Eagle": ["Talon", "Vision", "Summit", "Premier", "Medallion", "Wagon 3000 GT", "Spirit", "Summit Wagon", "Premier Wagon", "Talon TSi", "Talon TSi AWD"],
    "FAW": ["Talon", "Vision", "Summit", "Premier", "Medallion", "Wagon 3000 GT", "Spirit", "Summit Wagon", "Premier Wagon", "Talon TSi", "Talon TSi AWD"],
    "Ferrari": ["812 Superfast", "812 GTS", "F8 Tributo", "F8 Spider", "SF90 Stradale", "SF90 Spider", "Portofino", "Portofino M", "Roma", "Monza SP1", "Monza SP2", "LaFerrari", "LaFerrari Aperta", "488 GTB", "488 Spider", "488 Pista", "488 Pista Spider", "California", "California T", "GTC4Lusso", "GTC4Lusso T", "599 GTB Fiorano", "599 GTO", "Enzo Ferrari", "Testarossa", "F50", "F40", "Dino 246", "Dino 308 GT4"],
    "Fiat": ["500", "500X", "500L", "Panda", "Punto", "Tipo", "Uno", "Fiorino", "Qubo", "Doblo", "Strada", "Toro", "Palio", "Siena", "Bravo", "Stilo", "Linea", "Multipla", "Ducato", "Scudo", "Seicento", "Ritmo", "124 Spider", "Coupé", "127", "128", "131", "132", "850", "900", "1100"],
    "Ford": ["Fiesta", "Fiesta ST", "EcoSport", "Puma", "Focus", "Focus ST", "Focus RS", "Mondeo", "Fusion", "Mustang", "Mustang GT", "Mustang Mach 1", "Mustang Shelby GT350", "Mustang Shelby GT500", "Mustang Bullitt", "Mustang Mach-E", "Mustang Boss 302", "Mustang Boss 429", "Mustang Cobra", "Mustang Cobra R", "Mustang California Special", "Mustang GT Premium", "Mustang GT Convertible", "Mustang EcoBoost", "Mustang Shelby GT-H", "Mustang GT Fastback", "Mustang Shelby Super Snake", "Bronco", "Bronco Sport", "Explorer", "Edge", "Escape", "Kuga", "Ranger", "Ranger Raptor", "Ranger King Ranch", "F-150", "F-150 Raptor", "F-150 King Ranch", "F-250", "F-250 King Ranch", "F-350", "F-350 King Ranch", "F-450 King Ranch", "F-550 King Ranch", "Super Duty King Ranch", "Transit", "Transit Connect", "Transit Custom", "Maverick", "Everest", "Tourneo Custom", "Galaxy", "S-Max", "Ka", "Ka+", "Cortina", "Orion", "Probe", "Thunderbird", "GT", "Cougar", "Ranchero"],
    "Foton": ["Tunland E3", "Tunland E5", "Tunland S", "Tunland Yutu 8", "Tunland Yutu 9", "Tunland V7", "Tunland V9", "Tunland FT Crew", "Tunland G7", "Tunland G9", "Aumark", "Aumark S", "Aumark C", "Aumark Flex", "Aumark E", "Cargo TM1", "TM3", "TM5", "TM7", "TM10", "MIDI", "MIDI Cargo Box", "MIDI LWB", "Wonder Simple Chasis", "Wonder Cabina Doble", "Wonder Cabina Simple", "Harabas TM300", "Harabas TM300 Dropside", "Harabas TM300 Cab Chassis", "Tornado", "Auman R", "Auman C", "Auman D", "Auman EST", "Auman ETX", "Auman GTL", "Forland", "Forland King Kong", "Forland Ruiwo", "Ollin Beyond", "Ollin MRI", "Ollin V", "Mars 7", "Mars 9", "Rowor ES3", "Rowor ES5", "Rowor ES7", "Rowor Q5", "Rowor Q9", "Rowor Dajingang ES3", "Rowor Dajingang ES5", "View G9", "View G7", "View V5", "View V3", "Gratour im6", "Gratour im8", "Gratour ix5", "Gratour ix7", "Toano", "Toano Grand V", "MP-X", "Xiangling M", "Xiangling V", "Smart Smurf E5", "Smart Smurf E7", "Transvan AUV New Directions", "AUV Panoramic Unlimited", "AUV Pioneer Europe", "AUV Green Earth", "iBlue EV", "iBlue EV Van"],
    "Geely": ["Cityray", "GX3 Pro", "Emgrand", "EX5", "Monjaro", "Okavango", "Starray", "Geometry C", "Geometry E", "Geometry M6", "Geometry A", "Geometry G6", "Geometry Firefly", "Geometry L6", "Geometry L7", "Geometry L8", "Geometry Starshine 6", "Geometry Starshine 8", "Geometry Starship 7", "Geometry M9", "Geometry E8", "Geometry E5", "Geometry E7", "Geometry E9", "Geometry E10"],
    "Genesis": ["G70", "G70 Shooting Brake", "G80", "G80 Black", "Electrified G80", "G90", "G90 Long Wheel Base", "G90 Long Wheel Base Black", "GV60", "GV70", "Electrified GV70", "GV80", "GV80 Coupe", "GV80 Coupe Black", "GV80 Black", "Genesis X Gran Coupe", "Genesis X Convertible", "Genesis Neolun Concept", "Genesis Magma", "Genesis X Snow Speedium Concept", "Genesis X Gran Berlinetta", "Genesis X Gran Racer", "Genesis GV80 Coupe Concept", "Genesis X Concept", "Genesis GMR-001 Hypercar"],
    "Geo": ["Metro", "Metro Convertible", "Metro 4WD", "Metro LSi", "Metro XFi", "Metro 2-Door", "Metro 4-Door", "Metro 5-Door"],
    "GMC": ["Acadia", "Canyon", "Envoy", "Envoy XL", "Jimmy", "Safari", "Savana", "Sierra", "Sierra 1500", "Sierra 2500", "Sierra 3500", "Sonoma", "Terrain", "Typhoon", "Yukon", "Yukon Denali", "Yukon XL", "Yukon XL Denali", "Denali"],
    "Gonow": ["Troy", "GX5", "GX6", "GA200", "GA500", "GA200S", "GA500S", "Starry", "Victory", "Chariot"],
    "Great Wall": ["Wingle 3", "Wingle 5", "Wingle 6", "Wingle 7", "Great Wall Poer", "Great Wall Cannon", "Great Wall Ora", "Great Wall Tank 300", "Great Wall Tank 500", "Great Wall Tank 800"],
    "Hafei": ["Lobo", "Zhongyi", "Minyi", "Ruiyi", "Saibao", "Songhuajiang", "Haifeng", "Haifeng II"],
    "Haima": ["Haima 1", "Haima 2", "Haima 3", "Haima 7", "Haima 8S", "Haima F7", "Haima S5", "Haima S7", "Haima M3", "Haima M6", "Haima Freema"],
    "Haval": ["Haval H1", "Haval H2", "Haval H2s", "Haval H4", "Haval H5", "Haval H6", "Haval H6 Coupe", "Haval H7", "Haval H8", "Haval H9", "Haval Jolion", "Haval F7", "Haval F7x", "Haval Chulian", "Haval Cool Dog"],
    "Heibao": ["Heibao Q5", "Heibao Q7", "Heibao T20", "Heibao T22", "Heibao T30", "Heibao T32", "Heibao T50", "Heibao T60"],
    "Higer": ["Higer KLQ", "Higer KLQ6129", "Higer KLQ6119", "Higer KLQ6119LQ", "Higer KLQ6109", "Higer KLQ6109LQ", "Higer KLQ6108", "Higer KLQ6107", "Higer KLQ6105", "Higer KLQ6103", "Higer KLQ6700", "Higer KLQ6800", "Higer KLQ6890", "Higer KLQ6900", "Higer KLQ6902", "Higer KLQ6901"],
    "Hino": ["300 Series", "500 Series", "700 Series", "Dutro", "Ranger", "Profia", "XL Series", "Hino 200", "Hino 300", "Hino 500", "Hino 600", "Hino 700"],
    "Honda": ["Accord", "Accord Crosstour", "Amaze", "BR-V", "City", "Civic", "Civic Type R", "Civic Si", "Clarity", "CR-V", "CR-Z", "CR-X", "HR-V", "HR-V e:HEV", "Jazz", "Fit", "Insight", "Integra", "N-One", "N-Box", "N-WGN", "Passport", "Pilot", "Ridgeline", "S660", "S2000", "Stream", "Stepwgn", "Vezel", "Zest", "Freed", "Mobilio", "Brio", "Elysion"],
    "Hummer": ["H1", "H2", "H2 SUT", "H3", "H3T", "EV"],
    "Hyundai": ["Accent", "Accent GL", "Accent GLS", "Accent SE", "Accent SEL", "Accent Blue", "Aura", "Aura SX", "Aura SX(O)", "Atos", "Bayon", "Creta", "Creta GL", "Creta GLS", "Creta Limited", "Creta SX", "i10", "i10 Era", "i10 Magna", "i10 Sportz", "i10 Asta", "i20", "i20 Magna", "i20 Sportz", "i20 Asta", "i30", "i30 Era", "i30 Sportz", "i30 Asta", "i40", "i40 Active", "i40 Premium", "i45", "i50", "Kona", "Kona E", "Kona Electric", "Kona N Line", "Santa Fe", "Santa Fe GLS", "Santa Fe Limited", "Santa Fe Ultimate", "Santa Fe XL", "Sonata", "Sonata GL", "Sonata GLS", "Sonata Limited", "Sonata Hybrid", "Elantra", "Elantra GL", "Elantra GLS", "Elantra SE", "Elantra Limited", "Tucson", "Tucson GL", "Tucson GLS", "Tucson Limited", "Tucson N Line", "Venue", "Venue S", "Venue SX", "Venue SX(O)", "Palisade", "Palisade SE", "Palisade SEL", "Palisade Limited", "Palisade Calligraphy", "Nexo", "Staria", "Staria Lounge", "H-1", "H-1 Cargo", "H-1 Travel", "H-100", "Porter", "Genesis G70", "Genesis G80", "Genesis G90", "Ioniq 5", "Ioniq 6", "Ioniq 7", "Veloster", "Veloster N", "Entourage", "Tiburon", "Azera", "Veracruz", "Coupe"],
    "Infiniti": ["Q30", "Q40", "Q50", "Q50 Eau Rouge", "Q60", "Q60 Red Sport 400", "QX30", "QX50", "QX55", "QX56", "QX60", "QX70", "QX80", "FX35", "FX37", "FX50", "M30", "M35", "M37", "M45", "M56", "G20", "G25", "G35", "G37", "EX35", "EX37", "Q45"],
    "International": ["Durastar", "Transtar", "WorkStar", "LoneStar", "ProStar", "TerraStar", "Scout", "Scout II", "4000 Series", "7000 Series", "9000 Series", "9900i", "PayStar", "LoadStar", "CXT"],
    "Isuzu": ["D-Max", "MU-X", "Trooper", "Rodeo", "Axiom", "Hombre", "N-Series", "F-Series", "Elf", "Forward", "Giga", "D-Max V-Cross", "D-Max X-Series", "Panther", "Fargo", "Gemini", "VehiCROSS"],
    "Iveco": ["Daily", "Eurocargo", "Stralis", "Trakker", "S-Way", "Magirus", "TurboDaily", "PowerDaily", "Zeta", "Massif"],
    "JAC": ["iEV6E", "iEV7S", "iEV7X", "iEV8", "J3", "J3 Turin", "J4", "J5", "J6", "J7", "S2", "S3", "S4", "S5", "S7", "T6", "T8", "T9", "Refine M2", "Refine M3", "Refine M4", "Refine M5", "Refine M6", "Refine S2", "Refine S3", "Refine S5", "Refine S7", "Sunray"],
    "Jaecoo": ["JAECOO 7", "JAECOO 7 SHS", "JAECOO 7 PHEV", "JAECOO 5", "JAECOO 5 BEV", "JAECOO 6 EV", "JAECOO 8", "JAECOO 8 SHS", "JAECOO 5 EV"],
    "Jaguar": ["XE Pure", "XE R-Dynamic", "XE S", "XF Prestige", "XF R-Dynamic", "XF S", "XJ Portfolio", "XJ Supercharged", "XJ Autobiography", "F-Pace Prestige", "F-Pace R-Dynamic", "F-Pace S", "F-Pace SVR", "E-Pace S", "E-Pace SE", "E-Pace R-Dynamic", "E-Pace HSE", "I-Pace EV400", "I-Pace EV320", "F-Type Coupe", "F-Type Convertible", "F-Type R", "F-Type SVR", "XK", "XKR", "XJR", "S-Type", "Mark X"],
    "Jeep": ["4xe High Altitude", "Latitude", "Limited", "Laredo", "Longitude", "Mojave", "Overland", "Rubicon", "Rubicon 392", "S", "Sahara", "Sport", "Sport S", "Summit", "Summit Reserve", "Trailhawk", "Willys", "Avenger", "Cherokee", "Compass", "Gladiator", "Grand Cherokee", "Grand Cherokee L", "Grand Wagoneer", "Grand Wagoneer L", "Recon", "Wagoneer", "Wrangler", "Wrangler Unlimited"],
    "Jetour": ["Dashing 1.5T", "Dashing 1.6T PHEV", "X50", "X70", "X70M", "X70 Plus 1.5T", "X70 Plus 1.6T Lux", "X70 Plus 1.6T Comfort", "X70 Plus 1.6T Luxury", "X70 Pro", "X70 C-DM", "X90", "X90 Plus", "X90 Pro", "X90 C-DM", "T1 Shanhai", "T1", "T2 Shanhai", "T2", "Traveller T2 Shanhai", "L6 Shanhai", "L7 Shanhai", "L7 Plus Shanhai", "L9 Shanhai", "G700", "G900", "F700", "Reaolq Zongheng G600", "Zongheng G700", "Zongheng G900", "Zongheng F700"],
    "Jinbei": ["750", "A7", "A9", "Aurora", "Cargo Van", "Granse", "Haise", "H2", "H2L", "Haise King", "New Express", "New Haise", "New Haise EV", "Qinwu", "S30", "S35", "S50", "SY1028", "V19", "V22", "X30"],
    "JMC": ["BaoWei", "Boarding", "Carrying", "Convey", "Conquer", "CV9", "Dadao", "E-Lushun", "E-Road", "E-Shunda", "Fushun", "Shunda", "Teshun", "Transporter", "Yuhu", "Yuhu 3", "Yuhu 5", "Yuhu 7", "Yuhu 9", "Yuhu EV", "Yusheng S350"],
    "Jonway": ["A380", "A380 EV", "Falcon", "Wuxing E4X3"],
    "Kaiyi": ["E-Qute 02/Shiyue", "E-Qute 04/Shiyue Mate", "E5/Xuandu", "Showjet", "Showjet Pro", "X3/Showjet (X3)", "X7/Kunlun", "V7"],
    "Kia": ["Bongo", "EV Carens", "Clavis EV", "Carnival", "Clavis EV (submodelo de Carens Clavis EV)", "EV2", "EV3", "EV4", "EV5", "EV6", "EV9", "Forté", "K3 EV", "K4", "K5", "Niro", "Niro EV", "Niro Plus", "Picanto", "Rio", "Seltos", "Soul", "Soul EV", "Sportage", "Telluride"],
    "Lada": ["110 sedan", "111 wagon", "112 hatchback", "1200/1300", "1500", "Classic 2105", "Classic 2104", "Classic 2107", "Cross (Granta Cross)", "Granta", "Granta liftback", "Granta sedan", "Granta Sport", "Iskra", "Kalina", "Kalina Cross", "Kalina hatchback", "Kalina NFR", "Kalina sedan", "Kalina Sport", "Largus", "Largus 5 Seater", "Largus 7 Seater", "Largus Commercial", "Largus Cross 5 Seater", "Largus Cross 7 Seater", "Niva Legend", "Niva Travel", "Niva 4x4 3-doors", "Niva 4x4 5-doors", "Niva Urban 4x4 3-doors", "Niva Urban 4x4 5-doors", "Oka", "Priora", "Priora coupé", "Priora hatchback", "Priora sedan", "Priora wagon", "Riva 1500", "Samara", "Samara hatchback 3-doors", "Samara hatchback 5-doors", "Samara sedan", "Samara 2 hatchback 3-doors", "Samara 2 hatchback 5-doors", "Samara 2 sedan", "Vesta", "Vesta Cross", "Vesta NG", "Vesta sedan", "Vesta SW", "Vesta SW Cross", "Vesta SW Cross Black Limited Edition", "XRAY", "Aura"],
    "Lamborghini": ["350 GT", "400 GT", "Aventador", "Countach", "Diablo", "Espada S1", "Espada S2", "Espada S3", "Gallardo", "Huracán", "Huracán EVO", "Huracán Sterrato", "Huracán STO", "Huracán Tecnica", "Islero", "Jarama", "LM002", "Miura", "Murciélago", "Sian FKP 37", "Sian FKP 37 Strada", "Temerario", "Urus", "Urus Performante", "Urus SE", "Revuelto"],
    "Lancia": ["12 HP Alfa", "15/20 HP Beta", "18 HP Dialfa", "20 HP Gamma", "20/30 HP Delta", "20/30 HP Epsilon", "25-35 HP Theta", "A112", "A112 Abarth", "Appia", "Aurelia B20", "Aurelia B24 Spider", "Augusta", "Ardea", "Artena", "Astura", "AprilIa", "Aurelia", "Auto e Appia? (?)", "Beta", "Beta Coupe", "Beta Montecarlo", "Beta Spider", "Dedra", "Dedra Station Wagon", "Delta", "Delta HPE", "Dilambda", "Flaminia Berlina", "Flaminia Coupé", "Flaminia", "Flavia Convertible", "Flavia Sedan", "Flavia", "Fulvia Berlina", "Fulvia Coupe", "Gamma", "Gamma Coupe", "Kappa", "Kappa Coupe", "Kappa SW", "Lambda", "Lybra", "Lybra SW", "Musa", "Montecarlo / Scorpion", "Phedra", "Prisma", "Stratos", "Stratos (HF)", "Stratos Zero (concept)", "Thema", "Thesis", "Theta", "Trevi", "Voyager", "Y10", "Ypsilon", "Zeta"],
    "Land Rover": ["Defender 90", "Defender 110", "Defender 130", "Defender Hard Top", "Discovery", "Discovery 1", "Discovery 2", "Discovery 3 (LR3)", "Discovery 4 (LR4)", "Discovery 5", "Discovery Sport", "Discovery Sport S", "Discovery Sport SE", "Discovery Sport HSE", "Discovery Sport R-Dynamic", "Discovery Sport Metropolitan", "Freelander", "Freelander 1", "Freelander 2", "Range Rover", "Range Rover Classic", "Range Rover P38", "Range Rover L322", "Range Rover L405", "Range Rover L460", "Range Rover SVAutobiography", "Range Rover Westminster", "Range Rover Vogue", "Range Rover Evoque", "Evoque 3-door", "Evoque 5-door", "Evoque Convertible", "Evoque R-Dynamic", "Evoque Autobiography", "Range Rover Sport", "Sport L320", "Sport L494", "Sport L461", "Sport SVR", "Sport Autobiography", "Sport Dynamic SE", "Sport Dynamic HSE", "Range Rover Velar", "Velar S", "Velar SE", "Velar HSE", "Velar R-Dynamic", "Velar Autobiography", "Series", "Series I", "Series II", "Series IIA", "Series III"],
    "Lexus": ["Lexus LBX", "Lexus UX", "Lexus NX", "Lexus RX", "Lexus RX 500h", "Lexus RX 450h+", "Lexus RX 450h+ Plug-in Hybrid", "Lexus GX", "Lexus LX", "Lexus TX", "Lexus TX 350", "Lexus TX 500h", "Lexus TX 550h+", "Lexus RZ", "Lexus ES", "Lexus ES Hybrid", "Lexus ES 350e", "Lexus ES 500e", "Lexus IS", "Lexus IS 500", "Lexus IS 500 F SPORT", "Lexus LS", "Lexus LS Hybrid", "Lexus RC", "Lexus RC F", "Lexus RC F Track Edition", "Lexus LC", "Lexus LC Convertible", "Lexus LC Hybrid", "Lexus LM", "Lexus LM 350h", "Lexus LM 500h", "LEXUS LFA"],
    "Lifan": ["Lifan 320", "Lifan 330", "Lifan 520", "Lifan 520i", "Lifan 530", "Lifan 620", "Lifan 630", "Lifan 650", "Lifan 720", "Lifan 820", "Lifan 820EV", "Lifan Foison", "Lifan Foison Cargo", "Lifan Foison Truck", "Lifan X50", "Lifan X50 II", "Lifan X60", "Lifan X70", "Lifan X7", "Lifan X7 My Way", "Lifan X7 EX", "Lifan X7 SX", "Lifan X7 1.8L", "Lifan X7 1.8 My Way", "Lifan X7 1.8 EX", "Lifan X7 1.8 SX", "Lifan X70 2.0L"],
    "Lincoln": ["Lincoln Corsair", "Corsair Grand Touring", "Lincoln Nautilus", "Nautilus Grand Touring", "Lincoln Aviator", "Aviator Grand Touring", "Aviator Black Label", "Aviator Grand Touring Black Label", "Lincoln Navigator", "Navigator L", "Navigator Black Label", "Navigator L Black Label", "Lincoln Continental", "MKZ", "MKZ Hybrid", "Zephyr", "L Series", "LS", "Lincoln Blackwood", "Mark LT", "Futura", "Sentinel", "MK9", "Continental Concept 90", "Continental Concept 100", "Quicksilver", "Ghia", "Vignale", "Marque X", "Contempra", "L2K", "Special LS", "Navicross", "Mark X", "MKR", "C", "MKC", "Star", "Model L100"],
    "Lotus": ["Lotus Emira", "Emira V6", "Emira V6 First Edition", "Emira 4-Cylinder", "Emira 4-Cylinder First Edition", "Lotus Eletre", "Eletre R", "Eletre S", "Lotus Emeya", "Lotus Evija", "Lotus 3-Eleven", "3-Eleven 400", "3-Eleven 430", "Lotus Elise", "Elise S", "Elise Cup 250", "Elise Sprint", "Elise Sport 190", "Elise 220", "Elise 250", "Elise 260", "Elise 111S", "Elise R", "Lotus Exige", "Exige S", "Exige Cup 260", "Exige Sport 410", "Exige Sport 350", "Exige 240", "Exige GT3", "Exige 265E", "Exige Sprint", "Exige Club Racer", "Lotus Evora", "Evora 400", "Evora GT", "Evora 410", "Evora 430", "Evora 2+2", "Lotus 2-Eleven", "2-Eleven 300", "2-Eleven 340", "Lotus Europa S", "Europa S", "Europa SE", "Lotus 340R", "340R", "Lotus Elan", "Elan S2", "Elan Sprint", "Elan Plus 2", "Elan M100", "Lotus Esprit", "Esprit Turbo", "Esprit V8", "Esprit S4", "Esprit S4s", "Esprit GT3", "Esprit GT2", "Lotus Elite", "Elite Type 14", "Elite Type 75", "Lotus Éclat", "Éclat S2", "Éclat 2.2", "Lotus Excel", "Excel SE", "Excel 2.2", "Lotus Seven", "Super Seven", "Super Seven 1600", "Super Seven 1800", "Super Seven 2000", "Lotus Mark VI"],
    "Magiruz": ["Q30", "Q40", "Q50", "Q50 Eau Rouge", "Q60", "Q60 Red Sport 400", "QX30", "QX50", "QX55", "QX56", "QX60", "QX70", "QX80", "FX35", "FX37", "FX50", "M30", "M35", "M37", "M45", "M56", "G20", "G25", "G35", "G37", "EX35", "EX37", "Q45"],
    "Mahindra": ["Mahindra Thar", "Thar Roxx", "Thar.e", "Mahindra Scorpio", "Scorpio N", "Scorpio Classic", "Mahindra XUV700", "XUV700 Facelift", "Mahindra XUV400", "Mahindra XUV3XO", "XUV3XO EV", "Mahindra BE 6", "BE 6 Batman Edition", "Mahindra XEV 9e", "XEV 7e", "Mahindra Bolero", "Bolero 2025", "Bolero Neo", "Bolero Neo Plus", "Bolero Camper", "Mahindra Marazzo", "Mahindra Vision S", "Vision T", "Vision SXT", "Vision X", "Mahindra Maxx City", "Maxx HD", "Mahindra Veero", "Mahindra Pik-Up", "Mahindra Pik-Up Global", "Mahindra Pik-Up Camper", "Mahindra Pik-Up 4x4", "Mahindra Pik-Up 4x2", "Mahindra Pik-Up Chasis Cabina", "Mahindra Pik-Up Doble Cabina", "Mahindra Pik-Up Simple Cabina", "Mahindra Pik-Up 4x4 Doble Cabina", "Mahindra Pik-Up 4x2 Doble Cabina"],
    "Maserati": ["Maserati 4CL", "Maserati 4CLT", "Maserati 8CTF", "Maserati A6", "Maserati A6G", "Maserati A6GCS", "Maserati A6G/2000", "Maserati 3500 GT", "Maserati 3500 GTI", "Maserati 3500 GTI Spyder", "Maserati Mistral", "Maserati Mistral Spyder", "Maserati Merak", "Maserati Merak SS", "Maserati Bora", "Maserati Khamsin", "Maserati Kyalami", "Maserati Quattroporte", "Maserati Ghibli", "Maserati Ghibli (1992)", "Maserati Indy", "Maserati Biturbo", "Maserati 222", "Maserati 222 E", "Maserati 222 S", "Maserati 2.24v", "Maserati 2.24v E", "Maserati 2.24v S", "Maserati 2.8v", "Maserati 2.8v E", "Maserati 2.8v S", "Maserati 3200 GT", "Maserati Coupé", "Maserati Spyder", "Maserati GranSport", "Maserati GranTurismo", "Maserati GranCabrio", "Maserati Alfieri", "Maserati Levante", "Maserati Ghibli (2013)", "Maserati Quattroporte (2013)", "Maserati Grecale", "Maserati MC20", "Maserati MC20 Cielo", "Maserati GranTurismo (2023)", "Maserati GranCabrio (2023)", "Maserati GT2 Stradale", "Maserati MCPura", "Maserati MCPura Cielo", "Maserati MCXtrema", "Maserati Grecale Folgore", "Maserati GranTurismo Folgore", "Maserati GranCabrio Folgore"],
    "Maxus": ["T60 Línea Trabajo", "T60", "T90", "D90", "EG50", "D60", "EUNIQ 6", "G10 PANEL", "DELIVER 9 PANEL", "EV30", "V80", "V90", "G10"],
    "Mazda": ["Mazda2", "Mazda2 Hybrid", "Mazda3", "Mazda3 Hatchback", "Mazda3 Sedán", "Mazda6", "Mazda MX-5 Miata", "Mazda MX-5 RF", "Mazda MX-30", "Mazda MX-30 EV", "Mazda CX-3", "Mazda CX-4", "Mazda CX-5", "Mazda CX-50", "Mazda CX-50 Hybrid", "Mazda CX-60", "Mazda CX-70", "Mazda CX-70 PHEV", "Mazda CX-80", "Mazda CX-90", "Mazda CX-90 PHEV", "Mazda EZ-6", "Mazda EZ-60", "Mazda BT-50", "Mazda Bongo"],
    "Mercedes Benz": ["A-Class", "A-Class Sedan", "A-Class Hatchback", "A-Class Electric", "AMG A 35", "AMG A 45", "AMG A 45 S", "AMG A 45 E", "AMG A 45 S E", "B-Class", "B-Class Electric", "B-Class Electric Drive", "CLA-Class", "CLA Sedan", "CLA Shooting Brake", "AMG CLA 35", "AMG CLA 45", "AMG CLA 45 S", "CLE Coupe", "CLE Cabriolet", "AMG CLE 43", "AMG CLE 53", "C-Class", "C-Class Sedan", "C-Class Coupe", "C-Class Cabriolet", "C-Class Wagon", "AMG C 43", "AMG C 63", "AMG C 63 S", "AMG C 63 E", "AMG C 63 S E", "E-Class", "E-Class Sedan", "E-Class Coupe", "E-Class Cabriolet", "E-Class Wagon", "AMG E 43", "AMG E 53", "AMG E 63", "AMG E 63 S", "AMG E 63 S E", "S-Class", "S-Class Sedan", "S-Class Coupe", "S-Class Cabriolet", "S-Class Maybach", "AMG S 63", "AMG S 65", "AMG S 63 E", "AMG S 65 E", "G-Class", "G-Class SUV", "AMG G 63", "AMG G 65", "GLA-Class", "GLA SUV", "GLA Coupe", "AMG GLA 35", "AMG GLA 45", "AMG GLA 45 S", "GLB-Class", "GLB SUV", "GLB Coupe", "AMG GLB 35", "GLC-Class", "GLC SUV", "GLC Coupe", "AMG GLC 43", "AMG GLC 63", "AMG GLC 63 S", "AMG GLC 63 S E", "GLE-Class", "GLE SUV", "GLE Coupe", "AMG GLE 43", "AMG GLE 53", "AMG GLE 63", "AMG GLE 63 S", "AMG GLE 63 S E", "GLS-Class", "GLS SUV", "AMG GLS 63", "AMG GLS 63 S", "AMG GLS 63 S E", "EQB-Class", "EQB SUV", "EQB Electric", "EQE-Class", "EQE Sedan", "EQE SUV", "EQE Electric", "AMG EQE 43", "AMG EQE 53", "EQS-Class", "EQS Sedan", "EQS SUV", "EQS Electric", "AMG EQS 53", "AMG EQS 53 S", "AMG EQS 53 S E", "EQC-Class", "EQC SUV", "EQC Electric", "EQV-Class", "EQV Van", "EQV Electric", "EQT-Class", "EQT Van", "EQT Electric", "AMG EQT 43", "AMG EQT 53", "AMG EQT 63", "AMG EQT 63 S", "AMG EQT 63 S E", "AMG GT", "AMG GT Coupe", "AMG GT Roadster", "AMG GT Black Series", "AMG GT4", "AMG GT3", "AMG GT Track Series", "AMG SL", "AMG SL Roadster", "AMG SL Maybach", "AMG SL 43", "AMG SL 55", "AMG SL 63", "AMG SL 65", "AMG SLC 43", "AMG SLK", "AMG SLS AMG", "AMG SLS AMG Black Series", "AMG SLS AMG GT"],
    "Mercury": ["Marauder", "Sable", "Cougar", "Capri", "Villager", "Mariner", "Park Lane", "Commuter", "Mountaineer", "Milan", "Grand Marquis", "Mystique", "Comet", "Cyclone", "Monterey", "Montego", "Colony Park", "Montclair"],
    "MG": ["MG 3", "MG 5", "MG 5 EV", "MG 6", "MG 7", "MG 8", "MG GT", "MG GS", "MG HS", "MG HS Plug-in Hybrid", "MG HS EV", "MG ZS", "MG ZS EV", "MG Hector", "MG Hector Plus", "MG Marvel R", "MG Marvel X", "MG RX5", "MG RX8", "MG Cyberster"],
    "Mini Cooper": ["MINI 3 Door", "MINI 5 Door", "MINI Convertible", "MINI Clubman", "MINI Countryman", "MINI Electric / MINI Cooper SE", "MINI John Cooper Works 3 Door", "MINI John Cooper Works 5 Door", "MINI John Cooper Works Convertible", "MINI John Cooper Works Clubman", "MINI John Cooper Works Countryman"],
    "Mitsubishi": ["Mitsubishi Airtrek", "Mitsubishi ASX", "Mitsubishi Attrage", "Mitsubishi Eclipse Cross", "Mitsubishi Endeavor", "Mitsubishi L200", "Mitsubishi L300", "Mitsubishi Lancer", "Mitsubishi Lancer Evolution", "Mitsubishi Mirage", "Mitsubishi Montero", "Mitsubishi Montero Sport", "Mitsubishi Outlander", "Mitsubishi Outlander PHEV", "Mitsubishi Pajero", "Mitsubishi Pajero Sport", "Mitsubishi Triton", "Mitsubishi Xpander"],
    "Nissan": ["Altima", "Altima S", "Altima SV", "Altima SR", "Altima SL", "Altima Platinum", "Ariya", "Armada", "Frontier", "GT-R", "GT-R NISMO", "Kicks", "LEAF", "Murano", "Pathfinder", "Rogue", "Rogue Sport", "Sentra", "Titan", "Versa", "Z", "Z NISMO"],
    "Oldsmobile": ["4-4-2", "88", "Alero", "Aurora", "Bravada", "Custom Cruiser", "Cutlass", "Cutlass Ciera", "Cutlass Supreme", "Delta 88", "Intrigue", "Silhouette", "Toronado"],
    "Omoda": ["Omoda 5", "Omoda 5 BX", "Omoda 5 EX", "Omoda 5 EX+", "Omoda 5 GT", "Omoda 5 GT AWD", "Omoda E5", "Omoda C5", "Omoda C5 EV", "Omoda C7", "Omoda C9", "Omoda S5", "Omoda S5 EV", "Omoda S5 GT"],
    "Opel": ["Corsa", "Corsa Electric", "Astra", "Astra Electric", "Grandland", "Grandland Electric", "Mokka", "Mokka Electric", "Crossland", "Frontera", "Insignia", "Zafira Life", "Combo Life", "Vivaro"],
    "Peugeot": ["208", "208 Electric", "2008", "2008 Electric", "308", "308 Electric", "308 SW", "408", "408 Electric", "5008", "5008 Electric", "508", "508 Electric", "508 SW", "3008", "3008 Electric", "3008 Hybrid", "Rifter", "Traveller", "Partner", "Expert", "Boxer", "Landtrek"],
    "Piaggio": ["Ape", "Beverly 300", "Beverly 400", "Liberty 50", "Liberty 125", "Liberty 150", "Medley 125", "MP3 300 HPE", "MP3 500 HPE", "Typhoon 50", "Typhoon 125", "Zip 50"],
    "Plymouth": ["Acclaim", "Barracuda", "Belvedere", "Breeze", "Duster", "Fury", "GTX", "Neon", "Prowler", "Road Runner", "Satellite", "Valiant", "Voyager"],
    "Polarsun": ["Century", "Light Bus", "MPV-A", "MPV-B", "SUV", "Limousine", "Family MPV", "Business MPV"],
    "Porsche": ["718 Cayman", "718 Boxster", "911 Carrera", "911 Carrera S", "911 Turbo", "911 Turbo S", "911 GT3", "911 GT3 RS", "Taycan", "Taycan 4S", "Taycan Turbo", "Taycan Turbo S", "Panamera", "Panamera 4S", "Panamera Turbo", "Macan", "Macan S", "Macan GTS", "Macan Turbo", "Cayenne", "Cayenne S", "Cayenne GTS", "Cayenne Turbo"],
    "Proton": ["Saga", "Persona", "Iriz", "Exora", "S70", "X70", "X50", "X90", "e.MAS 7", "Satria", "Savvy", "Waja", "Preve", "Inspira", "Suprima S"],
    "Ram": ["1500 Tradesman", "1500 Big Horn", "1500 Laramie", "1500 Rebel", "1500 Limited", "1500 TRX", "2500 Tradesman", "2500 Big Horn", "2500 Laramie", "2500 Power Wagon", "3500 Tradesman", "3500 Big Horn", "3500 Laramie"],
    "Rambler": ["American", "American Custom", "American Super", "Classic", "Rebel", "Marlin"],
    "Renault": ["4", "5", "Clio", "Clio RS", "Mégane", "Mégane RS", "Twingo", "Twingo EV", "Captur", "Arkana", "Kadjar", "Koleos", "Austral", "Rafale", "Espace", "Kangoo", "Trafic", "Master", "Zoe", "5 E-Tech", "Kardian", "Duster", "Symbioz"],
    "Reva": ["REVAi", "Mahindra e2o", "Mahindra e2o Plus"],
    "Rivian": ["R1T", "R1S", "R2", "R3", "R3X"],
    "Rolls Royce": ["Phantom", "Ghost", "Wraith", "Dawn", "Cullinan", "Spectre", "Boat Tail", "Sweptail"],
    "Rover": ["25", "45", "75", "200", "400", "600", "800", "Metro"],
    "Saab": ["92", "93", "95", "96", "99", "900", "900 Turbo", "9000", "9-3", "9-5"],
    "Samsung": ["SM3", "SM5", "SM6", "SM7", "XM3", "QM3", "QM5", "QM6"],
    "Saturn": ["S-Series", "L-Series", "Ion", "Vue", "Outlook", "Aura", "Sky"],
    "Scion": ["xA", "xB", "xD", "tC", "iQ", "FR-S", "iM", "iA"],
    "Seat": ["Ibiza", "Leon", "Ateca", "Arona", "Tarraco", "Alhambra"],
    "Shinenay": ["X30", "X30L", "T20S", "T30S", "T50EV", "M7 Cargo"],
    "Skoda": ["Octavia", "Octavia RS", "Fabia", "Scala", "Kamiq", "Karoq", "Kodiaq", "Enyaq", "Superb"],
    "Smart": ["Fortwo", "Fortwo Cabrio", "Forfour", "EQ Fortwo", "EQ Forfour"],
    "Soueast": ["DX3", "DX5", "DX7", "DX9", "A5", "V3", "V5", "V6"],
    "Ssang Yong": ["Korando", "Musso", "Rexton", "Rodius", "Actyon", "Tivoli", "XLV"],
    "Subaru": ["Impreza", "Impreza WRX", "Impreza WRX STI", "Legacy", "Outback", "Forester", "BRZ", "Crosstrek", "XV", "Ascent"],
    "Suzuki": ["Alto", "Baleno", "Celerio", "Ertiga", "Swift", "Swift Sport", "Ignis", "Vitara", "Jimny", "S-Cross", "XL7"],
    "Tesla": ["Model S", "Model S Plaid", "Model 3", "Model 3 Performance", "Model X", "Model X Plaid", "Model Y", "Model Y Performance", "Cybertruck"],
    "Tiger Truck": ["Tiger Star", "Tiger Champ 4500", "Tiger Cub"],
    "Toyota": ["4Runner", "Corolla", "Corolla Cross", "Camry", "Hilux", "Land Cruiser", "Land Cruiser Prado", "RAV4", "Tacoma", "Tundra", "Sequoia", "Highlander", "Yaris", "GR86", "Supra", "GR Supra", "Yaris GR"],
    "VGV": ["U70", "U70 Pro", "U75 Plus", "VX7"],
    "Volkswagen": ["Amarok", "Arteon", "Beetle", "Golf", "Golf GTI", "Golf R", "Jetta", "Jetta GLI", "Passat", "Tiguan", "Tiguan Allspace", "Taos", "T-Cross", "ID.4", "ID.Buzz"],
    "Volvo": ["EX30", "EX40", "EX90", "XC40", "XC60", "XC90", "S90", "V90", "V90 Cross Country"],
    "Western Star": ["4700", "4800", "4900", "5700XE", "6900XD"],
    "Xpeng": ["G3i", "G6", "G9", "X9", "Mona M03"],
    "Yugo": ["45", "55", "GV", "Tempo"],
    "Zap": ["Alias Roadster", "Xebra"],
    "Zotye": ["2008", "5008", "5008 EV"],
    "Lucid": ["Air Pure", "Air Touring", "Air Grand Touring", "Air Sapphire", "Gravity"],
    "Neta": ["V", "V Pro", "U", "U Pro", "X", "Aya", "S"]
};

// Price and kilometer formatting functions (must be global for oninput handlers)
window.formatPrice = function (input) {
    const divisaSelect = document.querySelector('#divisa');
    const currencySymbol = divisaSelect && divisaSelect.value === 'USD' ? '$' : '₡';

    // Remove all non-digit characters except the currency symbol
    let value = input.value.replace(/[^\d]/g, '');

    if (value) {
        // Format with thousand separators using dots and prepend currency
        const formattedNumber = parseInt(value).toLocaleString('es-CR', {
            useGrouping: true,
            minimumFractionDigits: 0,
            maximumFractionDigits: 0
        }).replace(/,/g, '.');

        input.value = currencySymbol + formattedNumber;
    }
};

window.formatKilometer = function (input) {
    // Get the raw number value
    let value = input.value.replace(/[^\d]/g, '');
    if (value) {
        // Format with thousand separators using dots
        input.value = parseInt(value).toLocaleString('es-CR', {
            useGrouping: true,
            minimumFractionDigits: 0,
            maximumFractionDigits: 0
        }).replace(/,/g, '.');
    }
};

// Update price currency when divisa changes
window.updatePriceCurrency = function () {
    const precioInput = document.querySelector('#precio');
    if (precioInput && precioInput.value) {
        formatPrice(precioInput);
    }
};

document.addEventListener('DOMContentLoaded', function () {
    // Form navigation state
    let currentSection = 1;
    const totalSections = 8;

    // Inicializar selector de marca/modelo
    initializeMarcaModeloSelectors();

    // Initialize form
    initializeForm();

    function initializeForm() {
        showSection(1);
        updateSectionIndicator();
        initializeEventListeners();
        initializePaymentTabs();
        initializeFileUploads();
        initializeFAQ();
    }

    function initializeEventListeners() {
        // Navigation buttons
        const nextBtn = document.querySelector('#btn-next');
        const backBtn = document.querySelector('#btn-back');

        if (nextBtn) {
            // Remove any existing event listeners first
            nextBtn.removeEventListener('click', handleNextClick);

            // Add only ONE event listener
            nextBtn.addEventListener('click', handleNextClick);
        }

        if (backBtn) {
            backBtn.removeEventListener('click', handleBackClick);
            backBtn.addEventListener('click', handleBackClick);
        }

        // Modal informativo para el campo de placa
        const placaInput = document.querySelector('#placa');
        let placaModalShown = false;

        if (placaInput) {
            placaInput.addEventListener('focus', function () {
                if (!placaModalShown) {
                    showPlacaInfoModal();
                    placaModalShown = true;
                }
            });
        }

        // Form input handlers (sin validación en tiempo real)
        const formInputs = document.querySelectorAll('input, select, textarea');
        formInputs.forEach(input => {
            // Add character counter for description field
            if (input.name === 'Formulario.Descripcion') {
                input.addEventListener('input', function () {
                    updateCharacterCounter(this);
                });
                updateCharacterCounter(input); // Initialize counter
            }

            // Limpiar errores cuando el usuario empieza a corregir
            input.addEventListener('input', function () {
                clearFieldError(this);
            });

            input.addEventListener('change', function () {
                clearFieldError(this);
            });
        });

        // Equipment checkboxes
        const equipmentCheckboxes = document.querySelectorAll('.equipment-item input[type="checkbox"]');
        equipmentCheckboxes.forEach(checkbox => {
            checkbox.addEventListener('change', updateEquipmentSelection);
        });

        // Visibility plan selection - con funcionalidad de deselección
        const planRadios = document.querySelectorAll('input[name="Formulario.PlanVisibilidad"]');
        planRadios.forEach(radio => {
            // Agregar event listener al contenedor de la tarjeta para hacer clic en toda el área
            const planCard = radio.closest('.plan-option')?.querySelector('.plan-card');
            if (planCard) {
                planCard.addEventListener('click', function (e) {
                    // Si el click no fue en el radio button directamente
                    if (e.target !== radio) {
                        e.preventDefault();

                        // Si ya estaba seleccionado, deseleccionarlo
                        if (radio.checked) {
                            radio.checked = false;
                            // Actualizar UI para mostrar que no hay plan seleccionado
                            planCard.style.borderColor = 'rgba(255, 255, 255, 0.5)';
                            planCard.style.background = '#02081C';
                            planCard.style.boxShadow = 'none';
                        } else {
                            // Deseleccionar otros radios y sus tarjetas
                            planRadios.forEach(r => {
                                r.checked = false;
                                const otherCard = r.closest('.plan-option')?.querySelector('.plan-card');
                                if (otherCard) {
                                    otherCard.style.borderColor = 'rgba(255, 255, 255, 0.5)';
                                    otherCard.style.background = '#02081C';
                                    otherCard.style.boxShadow = 'none';
                                }
                            });

                            // Seleccionar este radio
                            radio.checked = true;
                            planCard.style.borderColor = '#00CC00';
                            planCard.style.background = 'rgba(0, 204, 0, 0.1)';
                            planCard.style.boxShadow = '0 0 20px rgba(0, 204, 0, 0.3)';
                        }

                        updatePaymentSummary();
                    }
                });
            }

            // Event listener para el radio button directo
            radio.addEventListener('change', function (e) {
                // Actualizar UI de todas las tarjetas
                planRadios.forEach(r => {
                    const card = r.closest('.plan-option')?.querySelector('.plan-card');
                    if (card) {
                        if (r.checked) {
                            card.style.borderColor = '#00CC00';
                            card.style.background = 'rgba(0, 204, 0, 0.1)';
                            card.style.boxShadow = '0 0 20px rgba(0, 204, 0, 0.3)';
                        } else {
                            card.style.borderColor = 'rgba(255, 255, 255, 0.5)';
                            card.style.background = '#02081C';
                            card.style.boxShadow = 'none';
                        }
                    }
                });

                updatePaymentSummary();
            });
        });

        // Tag radio buttons (solo uno puede ser seleccionado)
        const tagRadios = document.querySelectorAll('.tag-option input[type="radio"]');
        tagRadios.forEach(radio => {
            radio.addEventListener('change', updateTagSelection);

            // Agregar event listener al contenedor de la tarjeta para hacer clic en toda el área
            const tagCard = radio.closest('.tag-option').querySelector('.tag-card');
            if (tagCard) {
                tagCard.addEventListener('click', function (e) {
                    // Si el click no fue en el radio button directamente
                    if (e.target !== radio) {
                        radio.checked = !radio.checked;
                        radio.dispatchEvent(new Event('change'));
                    }
                });
            }
        });

        // Initialize tag videos
        initializeTagVideos();

        // Marca/Modelo cascade - DESACTIVADO: ahora se usa initializeMarcaModeloSelectors()
        // const marcaSelect = document.querySelector('#marca');
        // if (marcaSelect) {
        //     marcaSelect.addEventListener('change', updateModelos);
        // }

        // Province/canton cascade  
        const provinciaSelect = document.querySelector('#provincia');
        if (provinciaSelect) {
            provinciaSelect.addEventListener('change', function () {
                updateCantones(this);
            });
        }

        // Input formatting
        const precioInput = document.querySelector('#precio');
        if (precioInput) {
            precioInput.addEventListener('input', () => formatPrice(precioInput));
        }

        const kilometrajeInput = document.querySelector('#kilometraje');
        if (kilometrajeInput) {
            kilometrajeInput.addEventListener('input', () => formatKilometer(kilometrajeInput));
        }

        const cilindradaInput = document.querySelector('#cilindrada');
        if (cilindradaInput) {
            cilindradaInput.addEventListener('input', () => formatKilometer(cilindradaInput));
        }

        // File upload handlers
        const fileInputs = document.querySelectorAll('input[type="file"]');
        fileInputs.forEach(input => {
            input.addEventListener('change', handleFileUpload);
        });

        // Payment method toggle
        const paymentMethods = document.querySelectorAll('input[name="payment-method"]');
        paymentMethods.forEach(radio => {
            radio.addEventListener('change', togglePaymentMethod);
        });
    }

    async function handleNextClick(e) {
        e.preventDefault();

        // Mostrar estado de carga
        const nextBtn = document.querySelector('#btn-next');
        const originalText = nextBtn ? nextBtn.textContent : '';
        if (nextBtn) {
            nextBtn.disabled = true;
            nextBtn.textContent = 'Validando...';
        }

        const isValid = await validateCurrentSection();

        // Restaurar botón
        if (nextBtn) {
            nextBtn.disabled = false;
            nextBtn.textContent = originalText;
        }

        if (isValid) {
            if (currentSection < totalSections) {
                const previousSection = currentSection;
                currentSection++;

                showSection(currentSection);
                updateSectionIndicator();
                updateNavigationButtons();
            } else {
                // Final submission
                submitForm();
            }
        } else {
            // Hacer scroll hacia arriba para que el usuario vea el error
            window.scrollTo({ top: 0, behavior: 'smooth' });
        }
    }

    function handleBackClick(e) {
        e.preventDefault();

        if (currentSection > 1) {
            currentSection--;
            showSection(currentSection);
            updateSectionIndicator();
            updateNavigationButtons();
        }
    }



    function showSection(sectionNumber) {

        // Hide all sections first
        const sections = document.querySelectorAll('.form-section');
        sections.forEach(section => {
            section.classList.remove('active');
        });

        // Show the target section
        const currentSectionElement = document.querySelector(`#seccion${sectionNumber}`);
        if (currentSectionElement) {
            currentSectionElement.classList.add('active');
        } else {
        }

        updateNavigationButtons();
        updateSectionIndicator();

    }

    function updateSectionIndicator() {
        const indicator = document.querySelector('#current-section');
        if (indicator) {
            indicator.textContent = currentSection;
        }

        // Update section indicator in header - add space before /8
        const headerIndicator = document.querySelector('.section-indicator span');
        if (headerIndicator) {
            headerIndicator.textContent = currentSection;
        }
    }

    function updateNavigationButtons() {
        const nextBtn = document.querySelector('#btn-next');
        const backBtn = document.querySelector('#btn-back');

        if (backBtn) {
            backBtn.style.display = currentSection === 1 ? 'none' : 'flex';
        }

        if (nextBtn) {
            if (currentSection === totalSections) {
                nextBtn.textContent = 'Publicar anuncio';
            } else {
                nextBtn.textContent = 'Siguiente sección';
            }
        }
    }

    async function validateCurrentSection() {
        const currentSectionElement = document.querySelector(`#seccion${currentSection}`);
        if (!currentSectionElement) {
            return true;
        }

        const requiredFields = currentSectionElement.querySelectorAll('input[required], select[required], textarea[required]');

        let isValid = true;
        let missingFields = [];

        requiredFields.forEach(field => {
            const fieldName = field.name || field.id;
            const fieldValue = field.value ? field.value.trim() : '';

            // Verificar si el campo está vacío
            if (!fieldValue) {
                showFieldError(field, 'Este campo es requerido');
                isValid = false;

                // Obtener el label del campo para el modal
                const label = field.closest('[data-asterizco], [data-estado], [data-property-1]')?.querySelector('div:first-child')?.textContent?.trim() || fieldName;
                if (label && !missingFields.includes(label)) {
                    missingFields.push(label);
                }
            } 
            // Verificación especial para descripción: debe tener al menos 10 caracteres
            else if (field.id === 'descripcion' && fieldValue.length < 10) {
                showFieldError(field, 'La descripción debe tener al menos 10 caracteres');
                isValid = false;

                // Agregar al modal como campo faltante
                const label = 'Descripción detallada del vehículo (mínimo 10 caracteres)';
                if (!missingFields.includes(label)) {
                    missingFields.push(label);
                }
            } 
            else {
                clearFieldError(field);
            }
        });

        // Si hay campos faltantes, mostrar modal
        if (!isValid && missingFields.length > 0) {
            showMissingFieldsModal(missingFields);
        }

        // Section-specific validations
        switch (currentSection) {
            case 1: // Datos del vehículo
                const vehicleValid = await validateVehicleData();
                isValid = vehicleValid && isValid;
                break;
            case 2: // Equipamiento (opcional)
                // Equipment section is optional but must be shown to user
                // User must click "Next" to continue, regardless of selections
                isValid = true;
                break;
            case 3: // Ubicación
                const locationValid = validateLocationData();
                isValid = locationValid && isValid;
                break;
            case 6: // Pago
                const paymentValid = validatePaymentData();
                isValid = paymentValid && isValid;
                break;
            default:
                // For sections without specific validation, check only required fields
                break;
        }

        return isValid;
    }



    async function validateVehicleData() {
        const ano = document.querySelector('#ano');
        const kilometraje = document.querySelector('#kilometraje');
        const descripcion = document.querySelector('#descripcion');
        const precio = document.querySelector('#precio');
        const marca = document.querySelector('#marca');
        const modelo = document.querySelector('#modelo');
        const carroceria = document.querySelector('#carroceria');
        const combustible = document.querySelector('#combustible');
        const cilindrada = document.querySelector('#cilindrada');
        const colorExterior = document.querySelector('#color-exterior');
        const colorInterior = document.querySelector('#color-interior');
        const puertas = document.querySelector('#puertas');
        const pasajeros = document.querySelector('#pasajeros');
        const transmision = document.querySelector('#transmision');
        const traccion = document.querySelector('#traccion');
        const condicion = document.querySelector('#condicion');
        const placa = document.querySelector('#placa');

        let isValid = true;

        // Required field validations
        if (!marca || !marca.value) {
            if (marca) showFieldError(marca, 'La marca es requerida');
            isValid = false;
        } else {
            if (marca) clearFieldError(marca);
        }

        if (!modelo || !modelo.value) {
            if (modelo) showFieldError(modelo, 'El modelo es requerido');
            isValid = false;
        } else {
            if (modelo) clearFieldError(modelo);
        }

        // Year validation
        if (!ano || !ano.value) {
            if (ano) showFieldError(ano, 'El año es requerido');
            isValid = false;
        } else {
            const yearValue = parseInt(ano.value);
            const currentYear = new Date().getFullYear();
            if (yearValue < 1900 || yearValue > currentYear + 1) {
                showFieldError(ano, 'Ingrese un año válido');
                isValid = false;
            } else {
                clearFieldError(ano);
            }
        }

        if (!carroceria || !carroceria.value) {
            if (carroceria) showFieldError(carroceria, 'El tipo de carrocería es requerido');
            isValid = false;
        } else {
            if (carroceria) clearFieldError(carroceria);
        }

        if (!combustible || !combustible.value) {
            if (combustible) showFieldError(combustible, 'El tipo de combustible es requerido');
            isValid = false;
        } else {
            if (combustible) clearFieldError(combustible);
        }

        // Mileage validation
        if (!kilometraje || !kilometraje.value) {
            if (kilometraje) showFieldError(kilometraje, 'El kilometraje es requerido');
            isValid = false;
        } else {
            const kmValue = parseInt(kilometraje.value.replace(/\D/g, ''));
            if (kmValue < 0 || kmValue > 999999 || isNaN(kmValue)) {
                showFieldError(kilometraje, 'Ingrese un kilometraje válido');
                isValid = false;
            } else {
                clearFieldError(kilometraje);
            }
        }

        if (!cilindrada || !cilindrada.value) {
            if (cilindrada) showFieldError(cilindrada, 'La cilindrada es requerida');
            isValid = false;
        } else {
            if (cilindrada) clearFieldError(cilindrada);
        }

        if (!colorExterior || !colorExterior.value.trim()) {
            if (colorExterior) showFieldError(colorExterior, 'El color exterior es requerido');
            isValid = false;
        } else {
            if (colorExterior) clearFieldError(colorExterior);
        }

        if (!colorInterior || !colorInterior.value.trim()) {
            if (colorInterior) showFieldError(colorInterior, 'El color interior es requerido');
            isValid = false;
        } else {
            if (colorInterior) clearFieldError(colorInterior);
        }

        if (!puertas || !puertas.value) {
            if (puertas) showFieldError(puertas, 'El número de puertas es requerido');
            isValid = false;
        } else {
            if (puertas) clearFieldError(puertas);
        }

        if (!pasajeros || !pasajeros.value) {
            if (pasajeros) showFieldError(pasajeros, 'El número de pasajeros es requerido');
            isValid = false;
        } else {
            if (pasajeros) clearFieldError(pasajeros);
        }

        if (!transmision || !transmision.value) {
            if (transmision) showFieldError(transmision, 'El tipo de transmisión es requerido');
            isValid = false;
        } else {
            if (transmision) clearFieldError(transmision);
        }

        if (!traccion || !traccion.value) {
            if (traccion) showFieldError(traccion, 'El tipo de tracción es requerido');
            isValid = false;
        } else {
            if (traccion) clearFieldError(traccion);
        }

        if (!condicion || !condicion.value) {
            if (condicion) showFieldError(condicion, 'La condición del vehículo es requerida');
            isValid = false;
        } else {
            if (condicion) clearFieldError(condicion);
        }

        // Price validation
        if (!precio || !precio.value) {
            if (precio) showFieldError(precio, 'El precio es requerido');
            isValid = false;
        } else {
            // Remove currency symbol and dots to get the numeric value
            const precioValue = parseFloat(precio.value.replace(/[₡$.\s]/g, ''));
            if (precioValue <= 0 || isNaN(precioValue)) {
                showFieldError(precio, 'Ingrese un precio válido');
                isValid = false;
            } else {
                clearFieldError(precio);
            }
        }

        // Description validation
        if (!descripcion || !descripcion.value.trim()) {
            if (descripcion) showFieldError(descripcion, 'La descripción del vehículo es requerida');
            isValid = false;
        } else if (descripcion.value.trim().length < 10) {
            showFieldError(descripcion, 'La descripción debe tener al menos 10 caracteres');
            isValid = false;
        } else {
            clearFieldError(descripcion);
        }

        // NOTA: La validación de placa duplicada ha sido removida
        // Las placas pueden estar duplicadas en la base de datos

        return isValid;
    }

    function validateLocationData() {
        const provincia = document.querySelector('select[name="Formulario.Provincia"]');
        let isValid = true;


        if (!provincia || !provincia.value) {
            if (provincia) showFieldError(provincia, 'Seleccione una provincia');
            isValid = false;
        } else {
            if (provincia) clearFieldError(provincia);
        }

        return isValid;
    }

    function updateEquipmentSelection() {
        // Update equipment selection logic if needed
    }

    function updatePaymentSummary() {
        const selectedPlan = document.querySelector('input[name="Formulario.PlanVisibilidad"]:checked');
        const selectedTag = document.querySelector('.tag-option input[type="radio"]:checked');

        const serviceFee = 180; // Tarifa de servicio fija
        let planPrice = 0;
        let planName = "Ninguno";
        let tagPrice = 0;
        let tagName = "Ninguno";

        if (selectedPlan) {
            planPrice = parseFloat(selectedPlan.dataset.price || 0);
            planName = selectedPlan.dataset.planName || "Plan seleccionado";
        }

        // Solo un tag puede ser seleccionado con radio buttons
        if (selectedTag) {
            tagPrice = parseFloat(selectedTag.dataset.price || 0);
            tagName = selectedTag.dataset.tagName || "Banderín seleccionado";
        }

        const subtotal = planPrice + tagPrice;
        const iva = subtotal * 0.13; // 13% IVA
        const total = subtotal + iva + serviceFee;

        // Función helper para formatear números
        const formatCurrency = (value) => {
            return `₡${Math.round(value).toLocaleString('es-CR')}`;
        };

        // Update summary display con IDs específicos
        const summaryPlan = document.querySelector('#summary-plan');
        if (summaryPlan) {
            summaryPlan.querySelector('span:first-child').textContent = planName;
            summaryPlan.querySelector('span:last-child').textContent = formatCurrency(planPrice);
        }

        const summaryTag = document.querySelector('#summary-tag');
        if (summaryTag) {
            summaryTag.querySelector('span:first-child').textContent = tagName;
            summaryTag.querySelector('span:last-child').textContent = formatCurrency(tagPrice);
        }

        const summarySubtotal = document.querySelector('#summary-subtotal');
        if (summarySubtotal) {
            summarySubtotal.querySelector('span:last-child').textContent = formatCurrency(subtotal);
        }

        const summaryIva = document.querySelector('#summary-iva');
        if (summaryIva) {
            summaryIva.querySelector('span:last-child').textContent = formatCurrency(iva);
        }

        const summaryService = document.querySelector('#summary-service');
        if (summaryService) {
            summaryService.querySelector('span:last-child').textContent = formatCurrency(serviceFee);
        }

        const totalElement = document.querySelector('.total-amount');
        if (totalElement) {
            totalElement.textContent = formatCurrency(total);
        }
    }

    function updateSummaryLine(label, value) {
        const summaryLines = document.querySelectorAll('.summary-line');
        summaryLines.forEach(line => {
            const labelSpan = line.querySelector('span:first-child');
            if (labelSpan && labelSpan.textContent === label) {
                const valueSpan = line.querySelector('span:last-child');
                if (valueSpan) {
                    valueSpan.textContent = value;
                }
            }
        });
    }

    function updateSummaryTotal(total) {
        const totalElement = document.querySelector('.total-amount');
        if (totalElement) {
            totalElement.textContent = total;
        }
    }

    function initializePaymentTabs() {
        const paymentTabs = document.querySelectorAll('.payment-tab');
        const paymentContents = document.querySelectorAll('.payment-tab-content');

        paymentTabs.forEach(tab => {
            tab.addEventListener('click', function () {
                const targetTab = this.dataset.tab;

                // Remove active class from all tabs and contents
                paymentTabs.forEach(t => t.classList.remove('active'));
                paymentContents.forEach(c => c.classList.remove('active'));

                // Add active class to clicked tab and corresponding content
                this.classList.add('active');
                const targetContent = document.querySelector(`#${targetTab}`);
                if (targetContent) {
                    targetContent.classList.add('active');
                }
            });
        });
    }

    // Global array to store uploaded files
    let uploadedFiles = [];

    function initializeFileUploads() {
        const uploadAreas = document.querySelectorAll('.upload-area');

        uploadAreas.forEach((area, index) => {

            area.addEventListener('click', function () {
                const fileInput = document.createElement('input');
                fileInput.type = 'file';
                fileInput.accept = this.classList.contains('video') ? 'video/*' : 'image/*';
                fileInput.multiple = !this.classList.contains('video');

                fileInput.addEventListener('change', function (e) {
                    handleFileUpload(e.target.files, area);
                });

                fileInput.click();
            });

            // Drag and drop functionality
            area.addEventListener('dragover', function (e) {
                e.preventDefault();
                this.style.borderColor = 'white';
                this.style.background = 'rgba(255, 255, 255, 0.05)';
            });

            area.addEventListener('dragleave', function (e) {
                e.preventDefault();
                this.style.borderColor = 'rgba(255, 255, 255, 0.5)';
                this.style.background = '#02081C';
            });

            area.addEventListener('drop', function (e) {
                e.preventDefault();
                this.style.borderColor = 'rgba(255, 255, 255, 0.5)';
                this.style.background = '#02081C';

                handleFileUpload(e.dataTransfer.files, this);
            });
        });
    }

    function handleFileUpload(files, uploadArea) {

        const fileArray = Array.from(files);
        const isVideo = uploadArea.classList.contains('video');

        // Para videos: solo permitir 1 archivo
        if (isVideo && fileArray.length > 1) {
            alert('Solo se permite subir 1 video');
            return;
        }

        // Para imágenes: permitir hasta 10 archivos
        if (!isVideo) {
            const currentImages = uploadArea.querySelectorAll('.image-preview').length;
            if (currentImages + fileArray.length > 10) {
                alert('Solo se permiten máximo 10 imágenes');
                return;
            }
        }

        fileArray.forEach(file => {
            if (isVideo && !file.type.startsWith('video/')) {
                alert('Por favor, seleccione solo archivos de video');
                return;
            }

            if (!isVideo && !file.type.startsWith('image/')) {
                alert('Por favor, seleccione solo archivos de imagen');
                return;
            }

            // Create preview
            const reader = new FileReader();
            reader.onload = function (e) {
                if (isVideo) {
                    updateVideoPreview(uploadArea, e.target.result, file.name);
                } else {
                    addImagePreview(uploadArea, e.target.result, file.name, file);
                }
            };
            reader.readAsDataURL(file);
        });
    }

    function updateVideoPreview(uploadArea, src, fileName) {
        const placeholder = uploadArea.querySelector('.upload-placeholder');
        placeholder.innerHTML = `
            <video style="width: 100%; height: 100%; object-fit: contain; background: rgba(0,0,0,0.1);" controls>
                <source src="${src}" type="video/mp4">
            </video>
            <p style="position: absolute; bottom: 10px; left: 50%; transform: translateX(-50%); margin: 0; font-size: 12px; color: white; background: rgba(0,0,0,0.7); padding: 2px 8px; border-radius: 4px;">${fileName}</p>
        `;
    }

    function addImagePreview(uploadArea, src, fileName, file) {
        // Store file reference
        uploadedFiles.push(file);

        // NO crear previsualizaciones en el área de subida
        // Solo actualizar el texto del placeholder para indicar cuántas fotos se han subido
        const placeholder = uploadArea.querySelector('.upload-placeholder');
        if (placeholder) {
            const uploadText = placeholder.querySelector('p');
            if (uploadText) {
                uploadText.textContent = `${uploadedFiles.length} foto${uploadedFiles.length > 1 ? 's' : ''} subida${uploadedFiles.length > 1 ? 's' : ''}. Haga clic para agregar más.`;
                uploadText.style.color = '#00CC00';
            }
        }

        // Actualizar la sección de orden de fotos
        updatePhotoOrderSection();
    }

    function updatePhotoOrderSection() {
        const orderSection = document.querySelector('.photo-order-section');
        const uploadedGrid = document.querySelector('.uploaded-photos-grid');

        if (orderSection && uploadedGrid && uploadedFiles.length > 0) {
            orderSection.style.display = 'block';

            // Limpiar el grid
            uploadedGrid.innerHTML = '';

            // Agregar cada imagen con opción de seleccionar como principal
            uploadedFiles.forEach((file, index) => {
                const reader = new FileReader();
                reader.onload = function (e) {
                    const photoCard = document.createElement('div');
                    photoCard.className = 'uploaded-photo-card';
                    if (index === 0) {
                        photoCard.classList.add('principal');
                    }

                    photoCard.innerHTML = `
                        <div class="photo-preview">
                            <img src="${e.target.result}" alt="Foto ${index + 1}">
                            <button type="button" class="remove-photo-btn" data-index="${index}" style="
                                position: absolute;
                                top: 5px;
                                right: 5px;
                                width: 25px;
                                height: 25px;
                                border-radius: 50%;
                                background: rgba(255,0,0,0.9);
                                color: white;
                                border: none;
                                cursor: pointer;
                                font-size: 16px;
                                display: flex;
                                align-items: center;
                                justify-content: center;
                                transition: all 0.3s;
                                z-index: 10;
                            ">×</button>
                            <div class="photo-overlay">
                                <button type="button" class="set-principal-btn" data-index="${index}">
                                    ${index === 0 ? '⭐ Principal' : 'Establecer como principal'}
                                </button>
                            </div>
                        </div>
                        <div class="photo-info">
                            <span class="photo-number">Foto ${index + 1}</span>
                            ${index === 0 ? '<span class="principal-badge">Principal</span>' : ''}
                        </div>
                    `;

                    // Evento para establecer como principal
                    const setPrincipalBtn = photoCard.querySelector('.set-principal-btn');
                    setPrincipalBtn.addEventListener('click', function () {
                        // Reordenar el array de archivos
                        const selectedFile = uploadedFiles[index];
                        uploadedFiles.splice(index, 1);
                        uploadedFiles.unshift(selectedFile);

                        // Actualizar la visualización
                        updatePhotoOrderSection();

                        // Actualizar el contador en el área de subida
                        updateUploadAreaText();
                    });

                    // Evento para eliminar foto
                    const removeBtn = photoCard.querySelector('.remove-photo-btn');
                    removeBtn.addEventListener('click', function (e) {
                        e.stopPropagation();
                        uploadedFiles.splice(index, 1);
                        updatePhotoOrderSection();
                        updateUploadAreaText();
                    });

                    uploadedGrid.appendChild(photoCard);
                };
                reader.readAsDataURL(file);
            });
        } else if (orderSection) {
            orderSection.style.display = 'none';

            // Restaurar texto del placeholder si no hay fotos
            const uploadArea = document.querySelector('.upload-area.photos');
            if (uploadArea) {
                const placeholder = uploadArea.querySelector('.upload-placeholder');
                if (placeholder) {
                    const uploadText = placeholder.querySelector('p');
                    if (uploadText) {
                        uploadText.textContent = 'Haga clic o arrastre imágenes aquí';
                        uploadText.style.color = 'white';
                    }
                }
            }
        }
    }

    function validatePaymentData() {

        const aceptoTerminos = document.querySelector('#acepto-terminos');
        let isValid = true;

        // Verificar que se aceptaron los términos y condiciones
        if (!aceptoTerminos || !aceptoTerminos.checked) {
            if (aceptoTerminos) {
                showFieldError(aceptoTerminos, 'Debe aceptar los términos y condiciones para continuar');
            }
            showGlobalError('⚠️ Debe aceptar los términos y condiciones para continuar.');
            isValid = false;
        } else {
            if (aceptoTerminos) {
                clearFieldError(aceptoTerminos);
            }
            clearGlobalError();
        }

        return isValid;
    }

    function updateUploadAreaText() {
        const uploadArea = document.querySelector('.upload-area.photos');
        if (!uploadArea) return;

        const placeholder = uploadArea.querySelector('.upload-placeholder');
        if (!placeholder) return;

        const uploadText = placeholder.querySelector('p');
        if (!uploadText) return;

        if (uploadedFiles.length > 0) {
            uploadText.textContent = `${uploadedFiles.length} foto${uploadedFiles.length > 1 ? 's' : ''} subida${uploadedFiles.length > 1 ? 's' : ''}. Haga clic para agregar más.`;
            uploadText.style.color = '#00CC00';
        } else {
            uploadText.textContent = 'Haga clic o arrastre imágenes aquí';
            uploadText.style.color = 'white';
        }
    }

    function showFieldError(field, message) {
        clearFieldError(field);

        const errorDiv = document.createElement('div');
        errorDiv.className = 'field-error';
        errorDiv.textContent = message;
        errorDiv.style.color = '#FF0000';
        errorDiv.style.fontSize = '12px';
        errorDiv.style.marginTop = '4px';

        field.parentNode.appendChild(errorDiv);
        field.style.borderColor = '#FF0000';
    }

    function clearFieldError(field) {
        const existingError = field.parentNode.querySelector('.field-error');
        if (existingError) {
            existingError.remove();
        }
        field.style.borderColor = 'rgba(255, 255, 255, 0.5)';
    }

    function showGlobalError(message) {
        clearGlobalError();

        const errorDiv = document.createElement('div');
        errorDiv.className = 'global-error';
        errorDiv.textContent = message;
        errorDiv.style.cssText = `
            color: #FF0000;
            background: rgba(255, 0, 0, 0.1);
            border: 1px solid #FF0000;
            padding: 12px;
            border-radius: 4px;
            margin-bottom: 20px;
            text-align: center;
        `;

        const currentSectionElement = document.querySelector(`#seccion${currentSection}`);
        if (currentSectionElement) {
            currentSectionElement.insertBefore(errorDiv, currentSectionElement.firstChild);
        }
    }

    function clearGlobalError() {
        const existingError = document.querySelector('.global-error');
        if (existingError) {
            existingError.remove();
        }
    }

    function showPlacaInfoModal() {
        // Remover modal existente si hay uno
        const existingModal = document.querySelector('.placa-info-modal');
        if (existingModal) {
            existingModal.remove();
        }

        // Crear overlay
        const overlay = document.createElement('div');
        overlay.className = 'placa-info-modal';
        overlay.style.cssText = `
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(0, 0, 0, 0.7);
            display: flex;
            align-items: center;
            justify-content: center;
            z-index: 10000;
            animation: fadeIn 0.3s;
        `;

        // Crear modal
        const modal = document.createElement('div');
        modal.style.cssText = `
            background: #02081C;
            border: 2px solid #d7dcd7ff;
            border-radius: 12px;
            padding: 32px;
            max-width: 500px;
            width: 90%;
            box-shadow: 0 10px 40px rgba(190, 192, 190, 0.3);
            animation: slideIn 0.3s;
        `;

        // Icono
        const icon = document.createElement('div');
        icon.textContent = 'ℹ️';
        icon.style.cssText = `
            font-size: 48px;
            text-align: center;
            margin-bottom: 16px;
        `;

        // Título
        const title = document.createElement('h3');
        title.textContent = 'Información sobre la Placa del Vehículo';
        title.style.cssText = `
            color: #f2f2f4ff;
            font-size: 20px;
            font-family: Montserrat, sans-serif;
            font-weight: 700;
            margin: 0 0 16px 0;
            text-align: center;
        `;

        // Mensaje
        const message = document.createElement('p');
        message.textContent = 'El número de placa es únicamente para control interno de AutoClick.cr y NO será publicado en su anuncio público.';
        message.style.cssText = `
            color: rgba(255, 255, 255, 0.9);
            font-size: 15px;
            font-family: Montserrat, sans-serif;
            margin: 0 0 24px 0;
            line-height: 1.6;
            text-align: center;
        `;

        // Mensaje adicional
        const additionalInfo = document.createElement('p');
        additionalInfo.textContent = 'Gracias por su comprensión.';
        additionalInfo.style.cssText = `
            color: rgba(255, 255, 255, 0.7);
            font-size: 13px;
            font-family: Montserrat, sans-serif;
            margin: 0 0 24px 0;
            line-height: 1.5;
            text-align: center;
        `;

        // Botón de aceptar
        const acceptBtn = document.createElement('button');
        acceptBtn.textContent = 'Entendido';
        acceptBtn.style.cssText = `
            background: #1f1e44ff;
            color: white;
            border: none;
            border-radius: 6px;
            padding: 12px 32px;
            font-size: 15px;
            font-family: Montserrat, sans-serif;
            font-weight: 600;
            cursor: pointer;
            width: 100%;
            transition: all 0.3s;
        `;

        acceptBtn.addEventListener('mouseenter', () => {
            acceptBtn.style.background = '#00AA00';
            acceptBtn.style.transform = 'translateY(-2px)';
            acceptBtn.style.boxShadow = '0 4px 12px rgba(0, 204, 0, 0.4)';
        });

        acceptBtn.addEventListener('mouseleave', () => {
            acceptBtn.style.background = '#00CC00';
            acceptBtn.style.transform = 'translateY(0)';
            acceptBtn.style.boxShadow = 'none';
        });

        acceptBtn.addEventListener('click', () => {
            overlay.remove();
            // Enfocar el input después de cerrar el modal
            const placaInput = document.querySelector('#placa');
            if (placaInput) {
                placaInput.focus();
            }
        });

        // Cerrar al hacer click fuera del modal
        overlay.addEventListener('click', (e) => {
            if (e.target === overlay) {
                overlay.remove();
            }
        });

        // Agregar animaciones CSS si no existen
        if (!document.querySelector('#modal-animations')) {
            const style = document.createElement('style');
            style.id = 'modal-animations';
            style.textContent = `
                @keyframes fadeIn {
                    from { opacity: 0; }
                    to { opacity: 1; }
                }
                @keyframes slideIn {
                    from { 
                        transform: translateY(-50px);
                        opacity: 0;
                    }
                    to { 
                        transform: translateY(0);
                        opacity: 1;
                    }
                }
            `;
            document.head.appendChild(style);
        }

        // Ensamblar modal
        modal.appendChild(icon);
        modal.appendChild(title);
        modal.appendChild(message);
        modal.appendChild(additionalInfo);
        modal.appendChild(acceptBtn);
        overlay.appendChild(modal);

        // Agregar al DOM
        document.body.appendChild(overlay);
    }

    function showMissingFieldsModal(missingFields) {
        // Remover modal existente si hay uno
        const existingModal = document.querySelector('.missing-fields-modal');
        if (existingModal) {
            existingModal.remove();
        }

        // Crear overlay
        const overlay = document.createElement('div');
        overlay.className = 'missing-fields-modal';
        overlay.style.cssText = `
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(0, 0, 0, 0.7);
            display: flex;
            align-items: center;
            justify-content: center;
            z-index: 10000;
            animation: fadeIn 0.3s;
        `;

        // Crear modal
        const modal = document.createElement('div');
        modal.style.cssText = `
            background: #02081C;
            border: 2px solid #ff000087;
            border-radius: 8px;
            padding: 32px;
            max-width: 500px;
            width: 90%;
            box-shadow: 0 10px 40px rgba(255, 0, 0, 0.3);
            animation: slideIn 0.3s;
        `;

        // Título
        const title = document.createElement('h3');
        title.textContent = ' Campos Requeridos Faltantes';
        title.style.cssText = `
            color: #ff000087;
            font-size: 20px;
            font-family: Montserrat, sans-serif;
            font-weight: 700;
            margin: 0 0 16px 0;
            text-align: center;
        `;

        // Mensaje
        const message = document.createElement('p');
        message.textContent = 'Por favor, complete los siguientes campos antes de continuar:';
        message.style.cssText = `
            color: rgba(255, 255, 255, 0.9);
            font-size: 14px;
            font-family: Montserrat, sans-serif;
            margin: 0 0 16px 0;
        `;

        // Lista de campos
        const fieldList = document.createElement('ul');
        fieldList.style.cssText = `
            color: rgba(255, 255, 255, 0.8);
            font-size: 14px;
            font-family: Montserrat, sans-serif;
            margin: 0 0 24px 0;
            padding-left: 24px;
        `;

        missingFields.forEach(field => {
            const li = document.createElement('li');
            li.textContent = field;
            li.style.cssText = `
                margin-bottom: 8px;
            `;
            fieldList.appendChild(li);
        });

        // Botón de cerrar
        const closeBtn = document.createElement('button');
        closeBtn.textContent = 'Entendido';
        closeBtn.style.cssText = `
            background: #ff000087;
            color: white;
            border: none;
            border-radius: 4px;
            padding: 12px 24px;
            font-size: 14px;
            font-family: Montserrat, sans-serif;
            font-weight: 600;
            cursor: pointer;
            width: 100%;
            transition: all 0.3s;
        `;

        closeBtn.addEventListener('mouseenter', () => {
            closeBtn.style.background = '#CC0000';
        });

        closeBtn.addEventListener('mouseleave', () => {
            closeBtn.style.background = '#FF0000';
        });

        closeBtn.addEventListener('click', () => {
            overlay.remove();
        });

        // Cerrar al hacer click fuera del modal
        overlay.addEventListener('click', (e) => {
            if (e.target === overlay) {
                overlay.remove();
            }
        });

        // Agregar animaciones CSS
        const style = document.createElement('style');
        style.textContent = `
            @keyframes fadeIn {
                from { opacity: 0; }
                to { opacity: 1; }
            }
            @keyframes slideIn {
                from { 
                    transform: translateY(-50px);
                    opacity: 0;
                }
                to { 
                    transform: translateY(0);
                    opacity: 1;
                }
            }
        `;
        document.head.appendChild(style);

        // Ensamblar modal
        modal.appendChild(title);
        modal.appendChild(message);
        modal.appendChild(fieldList);
        modal.appendChild(closeBtn);
        overlay.appendChild(modal);

        // Agregar al DOM
        document.body.appendChild(overlay);
    }

    function updateCharacterCounter(textarea) {
        const currentLength = textarea.value.length;
        const minLength = 10;

        // Remove existing counter (buscar después del textarea)
        let existingCounter = textarea.nextElementSibling;
        if (existingCounter && existingCounter.classList.contains('character-counter')) {
            existingCounter.remove();
        }

        // Create new counter
        const counter = document.createElement('div');
        counter.className = 'character-counter';
        counter.style.cssText = `
            font-size: 12px;
            margin-top: 8px;
            color: ${currentLength >= minLength ? '#4CAF50' : '#FF9800'};
            position: absolute;
            left: 0px;
            bottom: -30px;
        `;
        counter.textContent = `${currentLength}/${minLength} caracteres mínimos`;

        // Insertar después del textarea
        textarea.parentNode.insertBefore(counter, textarea.nextSibling);
    }

    function submitForm() {

        // Show loading state
        const nextBtn = document.querySelector('.btn-next');
        if (nextBtn) {
            nextBtn.textContent = 'Procesando...';
            nextBtn.disabled = true;
        }

        const form = document.querySelector('#anuncioForm');
        if (!form) {
            return;
        }


        // Prepare all data before creating FormData
        prepareEquipmentData();
        prepareFileData();

        // Create FormData object which automatically handles files
        const formData = new FormData(form);

        // Add handler for the specific endpoint
        formData.append('handler', 'Finalizar');

        // Debug: log all FormData entries
        for (let [key, value] of formData.entries()) {
            if (value instanceof File) {
            } else {
            }
        }

        // Submit using fetch with FormData
        fetch(form.action || window.location.pathname, {
            method: 'POST',
            body: formData
        })
            .then(response => {
                if (response.ok) {
                    window.location.href = '/'; // Redirect on success
                } else {
                    alert('Error al enviar el formulario');
                }
            })
            .catch(error => {
                alert('Error de conexión');
            })
            .finally(() => {
                // Reset button state
                if (nextBtn) {
                    nextBtn.textContent = 'Finalizar';
                    nextBtn.disabled = false;
                }
            });
    }

    function prepareEquipmentData() {

        const form = document.querySelector('#anuncioForm');
        if (!form) {
            return;
        }

        // NO eliminar los checkboxes originales - FormData los capturará automáticamente
        // Solo verificar que existan
        const allExtrasCheckboxes = form.querySelectorAll('input[type="checkbox"][name*="Extras"]');

        const checkedExtras = form.querySelectorAll('input[type="checkbox"][name*="Extras"]:checked');

        checkedExtras.forEach(cb => {
        });

        // Los checkboxes ya tienen los nombres correctos en el HTML
        // FormData(form) los capturará automáticamente cuando estén checked
        // No necesitamos crear hidden inputs


        /* ===================================================================
         * CÓDIGO ANTERIOR DESHABILITADO
         * Ya no es necesario manipular los checkboxes porque:
         * 1. Los checkboxes HTML ya tienen los nombres correctos (Formulario.ExtrasExterior, etc.)
         * 2. FormData(form) captura automáticamente los checkboxes checked
         * 3. ASP.NET Core agrupa múltiples inputs con el mismo nombre en un array
         * 4. El método SerializarExtrasFromForm() en C# los serializa a JSON
         * =================================================================== */
    }

    function prepareFileData() {

        const fotosInput = document.getElementById('fotosInput');
        if (!fotosInput) {
            return;
        }

        if (uploadedFiles.length > 0) {

            // Create a new DataTransfer object to hold our files
            const dt = new DataTransfer();

            // Add each file to the DataTransfer
            uploadedFiles.forEach((file, index) => {
                dt.items.add(file);
            });

            // Assign the files to the input
            fotosInput.files = dt.files;

        } else {
        }

    }

    // Utility functions for form inputs
    // Removed duplicate formatPrice and formatKilometer functions - using the ones above

    // Province/Canton cascade
    window.updateCantones = function (provinciaSelect) {
        const cantonSelect = document.querySelector('#canton');
        const provincia = provinciaSelect.value;

        // Clear current options
        cantonSelect.innerHTML = '<option value="">Elige un cantón</option>';

        // Add cantones based on province
        const cantonesPorProvincia = {
            'San José': ['Central', 'Escazú', 'Desamparados', 'Puriscal', 'Tarrazú', 'Aserrí', 'Mora', 'Goicoechea', 'Santa Ana', 'Alajuelita', 'Coronado', 'Acosta', 'Tibás', 'Moravia', 'Montes de Oca', 'Turrubares', 'Dota', 'Curridabat', 'Pérez Zeledón', 'León Cortés'],
            'Alajuela': ['Central', 'San Ramón', 'Grecia', 'San Mateo', 'Atenas', 'Naranjo', 'Palmares', 'Poás', 'Orotina', 'San Carlos', 'Zarcero', 'Sarchí', 'Upala', 'Los Chiles', 'Guatuso'],
            'Cartago': ['Central', 'Paraíso', 'La Unión', 'Jiménez', 'Turrialba', 'Alvarado', 'Oreamuno', 'El Guarco'],
            'Heredia': ['Central', 'Barva', 'Santo Domingo', 'Santa Bárbara', 'San Rafael', 'San Isidro', 'Belén', 'Flores', 'San Pablo', 'Sarapiquí'],
            'Guanacaste': ['Liberia', 'Nicoya', 'Santa Cruz', 'Bagaces', 'Carrillo', 'Cañas', 'Abangares', 'Tilarán', 'Nandayure', 'La Cruz', 'Hojancha'],
            'Puntarenas': ['Central', 'Esparza', 'Buenos Aires', 'Montes de Oro', 'Osa', 'Quepos', 'Golfito', 'Coto Brus', 'Parrita', 'Corredores', 'Garabito'],
            'Limón': ['Central', 'Pococí', 'Siquirres', 'Talamanca', 'Matina', 'Guácimo']
        };

        if (cantonesPorProvincia[provincia]) {
            cantonesPorProvincia[provincia].forEach(canton => {
                const option = document.createElement('option');
                option.value = canton;
                option.textContent = canton;
                cantonSelect.appendChild(option);
            });
        }
    };

    // Marca/Modelo cascade
    function updateModelos() {
        const marcaSelect = document.querySelector('#marca');
        const modeloSelect = document.querySelector('#modelo');
        const marca = marcaSelect.value;

        // Clear current options
        modeloSelect.innerHTML = '<option value="">Elige un modelo</option>';

        // Add models based on brand
        const modelosPorMarca = {
            'toyota': ['Corolla', 'Camry', 'Prius', 'RAV4', 'Highlander', 'Tacoma', 'Tundra', '4Runner', 'Sienna', 'Avalon'],
            'honda': ['Civic', 'Accord', 'CR-V', 'Pilot', 'Fit', 'HR-V', 'Passport', 'Ridgeline', 'Insight', 'Odyssey'],
            'nissan': ['Sentra', 'Altima', 'Maxima', 'Rogue', 'Pathfinder', 'Murano', 'Frontier', 'Titan', 'Armada', 'Leaf'],
            'hyundai': ['Elantra', 'Sonata', 'Tucson', 'Santa Fe', 'Kona', 'Palisade', 'Accent', 'Veloster', 'Genesis', 'Ioniq'],
            'kia': ['Forte', 'Optima', 'Sportage', 'Sorento', 'Soul', 'Seltos', 'Telluride', 'Stinger', 'Niro', 'Carnival'],
            'mazda': ['Mazda3', 'Mazda6', 'CX-3', 'CX-5', 'CX-9', 'MX-5', 'CX-30', 'Mazda2', 'BT-50'],
            'chevrolet': ['Spark', 'Sonic', 'Cruze', 'Malibu', 'Equinox', 'Traverse', 'Silverado', 'Tahoe', 'Suburban'],
            'ford': ['Fiesta', 'Focus', 'Fusion', 'Escape', 'Explorer', 'F-150', 'Ranger', 'Expedition', 'Mustang'],
            'volkswagen': ['Jetta', 'Passat', 'Tiguan', 'Atlas', 'Golf', 'Beetle', 'Touareg', 'Arteon']
        };

        if (modelosPorMarca[marca]) {
            modelosPorMarca[marca].forEach(modelo => {
                const option = document.createElement('option');
                option.value = modelo.toLowerCase();
                option.textContent = modelo;
                modeloSelect.appendChild(option);
            });
        }
    }

    // Utility functions for form interactions
    // handleFileUpload function moved to avoid duplication - see line 748 for the main implementation

    function togglePaymentMethod() {
        const selectedMethod = document.querySelector('input[name="payment-method"]:checked');
        const paymentDetails = document.querySelectorAll('.payment-details');

        paymentDetails.forEach(detail => {
            detail.style.display = 'none';
        });

        if (selectedMethod) {
            const targetDetail = document.querySelector(`#${selectedMethod.value} -details`);
            if (targetDetail) {
                targetDetail.style.display = 'block';
            }
        }
    }

    function updateImageViewMode(container, mode) {
        if (!container) return;

        const images = container.querySelectorAll('.preview-image');
        const objectFitValue = mode === 'cover' ? 'cover' : 'contain';

        images.forEach(img => {
            img.style.objectFit = objectFitValue;
        });

    }




    function updatePlanSelection(event) {
        const radio = event.target;
        const planItems = document.querySelectorAll('.plan-item');

        planItems.forEach(item => {
            item.classList.remove('selected');
        });

        const selectedPlan = radio.closest('.plan-item');
        if (selectedPlan) {
            selectedPlan.classList.add('selected');
        }

        // Actualizar el resumen de pago cuando cambia el plan
        updatePaymentSummary();
    }

    function updateTagSelection(event) {
        const radio = event.target;
        const tagOptions = document.querySelectorAll('.tag-option');

        // Remover la clase 'selected' de todas las opciones
        tagOptions.forEach(option => {
            const tagCard = option.querySelector('.tag-card');
            if (tagCard) {
                tagCard.style.borderColor = 'rgba(255, 255, 255, 0.5)';
                tagCard.style.background = '#02081C';
                tagCard.style.boxShadow = 'none';
            }
        });

        // Agregar la clase 'selected' solo a la opción seleccionada
        const selectedTagOption = radio.closest('.tag-option');
        if (selectedTagOption && radio.checked) {
            const tagCard = selectedTagOption.querySelector('.tag-card');
            if (tagCard) {
                tagCard.style.borderColor = '#00CC00';
                tagCard.style.background = 'rgba(0, 204, 0, 0.1)';
                tagCard.style.boxShadow = '0 0 20px rgba(0, 204, 0, 0.3)';
            }
        }

        // Actualizar el resumen de pago cuando cambia el banderín
        updatePaymentSummary();
    }

    function initializeTagVideos() {
        const tagVideos = document.querySelectorAll('.tag-video');

        tagVideos.forEach((video, index) => {
            const videoSrc = video.getAttribute('data-src');

            if (videoSrc) {

                // Set sources on source elements if they exist
                const sources = video.querySelectorAll('source[data-src]');
                sources.forEach(source => {
                    const src = source.getAttribute('data-src');
                    if (src) {
                        source.src = src;
                    }
                });

                // Also set directly on video element as fallback
                video.src = videoSrc;

                // Add error handler
                video.addEventListener('error', function (e) {
                    // Show fallback text
                    const container = video.closest('.tag-image');
                    if (container) {
                        container.innerHTML = `
            < div style = "width: 100%; height: 100%; display: flex; flex-direction: column; align-items: center; justify-content: center; background: rgba(255,255,255,0.05); border-radius: 4px; padding: 10px;" >
                                <span style="color: rgba(255,255,255,0.5); font-size: 12px; text-align: center;">
                                    ðŸŽ¬ Video no disponible
                                </span>
                                <small style="color: rgba(255,255,255,0.3); font-size: 10px; text-align: center; margin-top: 4px;">
                                    Formato .MOV no soportado en este navegador
                                </small>
                            </div >
            `;
                    }
                }, { once: true });

                // Add success handler
                video.addEventListener('loadeddata', function () {
                }, { once: true });

                // Add loadstart handler
                video.addEventListener('loadstart', function () {
                });

                // Force video to load
                video.load();

                // Try to play with better error handling
                const playPromise = video.play();
                if (playPromise !== undefined) {
                    playPromise.then(() => {
                    }).catch(err => {

                        // If autoplay fails, try on user interaction
                        video.addEventListener('canplay', () => {
                            video.play().catch(e => {
                            });
                        }, { once: true });
                    });
                }
            }
        });

        // Log overall browser video support
        const testVideo = document.createElement('video');
        const supportInfo = {
            'MP4 (H.264)': testVideo.canPlayType('video/mp4; codecs="avc1.42E01E"'),
            'WebM': testVideo.canPlayType('video/webm; codecs="vp8, vorbis"'),
            'Ogg': testVideo.canPlayType('video/ogg; codecs="theora"'),
            'QuickTime MOV': testVideo.canPlayType('video/quicktime'),
            'MOV (H.264)': testVideo.canPlayType('video/mp4; codecs="avc1.42E01E"')
        };

        // Check if QuickTime is supported
        if (!supportInfo['QuickTime MOV'] || supportInfo['QuickTime MOV'] === '') {
        }
    }

    // Load banderines from Azure Blob Storage
    loadBanderinesFromBlobStorage();
});

async function loadBanderinesFromBlobStorage() {

    try {
        // Get all banderines with data-banderinfile attribute
        const banderinImages = document.querySelectorAll('img[data-banderinfile]');

        for (const img of banderinImages) {
            const fileName = img.getAttribute('data-banderinfile');
            if (fileName) {
                try {
                    const response = await fetch(`/api/banderines/${encodeURIComponent(fileName)}`);
                    if (response.ok) {
                        const url = await response.text(); // Controller returns URL as string
                        if (url && url.startsWith('http')) {
                            img.src = url.replace(/"/g, ''); // Remove any quotes
                        }
                    } else {
                        // Keep the image hidden on error
                    }
                } catch (error) {
                }
            }
        }

        // Get all logos with data-logofile attribute (different container)
        const logoImages = document.querySelectorAll('img[data-logofile]');

        for (const img of logoImages) {
            const fileName = img.getAttribute('data-logofile');
            if (fileName) {
                try {
                    // Use the logos container instead of banderines
                    const response = await fetch(`/api/banderines/logo/${encodeURIComponent(fileName)}`);
                    if (response.ok) {
                        const url = await response.text();
                        if (url && url.startsWith('http')) {
                            img.src = url.replace(/"/g, '');
                        }
                    } else {
                    }
                } catch (error) {
                }
            }
        }
    } catch (error) {
    }
}
// Funciï¿½n para inicializar los selectores de marca y modelo
function initializeMarcaModeloSelectors() {

    const marcaSelect = document.getElementById('marca');
    let modeloSelect = document.getElementById('modelo'); // Cambiar const a let

    if (!marcaSelect || !modeloSelect) {
        return;
    }


    if (typeof marcasModelos === 'undefined') {
        return;
    }


    // No need to set styles here - CSS handles it

    marcaSelect.addEventListener('change', function () {
        const marcaSeleccionada = this.value;

        const parentDiv = modeloSelect.parentElement;
        const oldSelect = modeloSelect;

        // Remover completamente el select viejo
        oldSelect.remove();

        // Crear un select completamente nuevo con MÍNIMOS estilos
        const newSelect = document.createElement('select');
        newSelect.id = 'modelo';
        newSelect.name = 'Formulario.Modelo';
        newSelect.required = true;
        newSelect.className = 'modelo-select-dynamic';

        // Solo aplicar los estilos ESENCIALES para posicionamiento y apariencia básica
        newSelect.style.cssText = `
        width: 260px;
        height: 42px;
        padding: 8px 16px;
        position: absolute;
        left: 0px;
        top: 18px;
        background: #02081C;
        border: 0.50px solid rgba(255, 255, 255, 0.50);
        border - radius: 4px;
        color: rgba(255, 255, 255, 0.50);
        font - size: 14px;
        font - family: Montserrat, sans - serif;
        font - weight: 500;
        cursor: pointer;
        `;

        if (marcaSeleccionada && marcasModelos[marcaSeleccionada]) {
            const modelos = marcasModelos[marcaSeleccionada];

            // Agregar opción por defecto
            const defaultOption = document.createElement('option');
            defaultOption.value = '';
            defaultOption.textContent = 'Elija un modelo';
            newSelect.appendChild(defaultOption);

            // Agregar todas las opciones de modelos
            modelos.forEach(function (modelo) {
                const option = document.createElement('option');
                option.value = modelo;
                option.textContent = modelo;
                newSelect.appendChild(option);
            });

            newSelect.disabled = false;

        } else {
            const defaultOption = document.createElement('option');
            defaultOption.value = '';
            defaultOption.textContent = 'Elija un modelo';
            newSelect.appendChild(defaultOption);
            newSelect.disabled = true;
        }


        // Insertar el nuevo select en el DOM
        parentDiv.appendChild(newSelect);


        // Actualizar la referencia global
        modeloSelect = newSelect;

        const computedStyles = window.getComputedStyle(newSelect);

        // Verificar opciones una vez más
        for (let i = 0; i < Math.min(5, newSelect.options.length); i++) {
        }

        // Intentar abrir el dropdown programáticamente para ver si funciona
        setTimeout(function () {
            newSelect.focus();
        }, 100);
    });

    if (marcaSelect.value) {
        marcaSelect.dispatchEvent(new Event('change'));
    }

}


// FAQ functionality
let faqInitialized = false; // Bandera para evitar inicialización múltiple

function initializeFAQ() {
    if (faqInitialized) {
        return;
    }

    const faqItems = document.querySelectorAll('.faq-item');

    if (faqItems.length === 0) {
        return;
    }

    faqItems.forEach((item, index) => {
        const question = item.querySelector('.faq-question');

        if (question) {
            question.addEventListener('click', function (e) {
                e.stopPropagation(); // Evitar que el evento se propague

                const isActive = item.classList.contains('active');

                // Close all FAQ items
                faqItems.forEach(faq => {
                    faq.classList.remove('active');
                });

                // Open clicked item if it wasn't active
                if (!isActive) {
                    item.classList.add('active');
                }
            });
        }
    });

    faqInitialized = true; // Marcar como inicializado
}

// Initialize FAQ when DOM is ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', function () {
        initializeFAQ();
    });
} else {
    initializeFAQ();
}

// Toggle para el desglose de pago en móvil
function togglePaymentSummary() {
    const toggle = document.querySelector('.payment-summary-toggle');
    const details = document.querySelector('.payment-summary-details');

    if (toggle && details) {
        toggle.classList.toggle('active');
        details.classList.toggle('expanded');
    }
}

// Sincronizar el precio en el toggle móvil con el precio total
function updateMobileTotalDisplay() {
    const totalAmount = document.querySelector('.total-amount');
    const totalAmountMobile = document.querySelector('.total-amount-mobile');

    if (totalAmount && totalAmountMobile) {
        totalAmountMobile.textContent = totalAmount.textContent;
    }
}

// Observar cambios en el precio total para actualizar el toggle móvil
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', function () {
        const totalAmount = document.querySelector('.total-amount');
        if (totalAmount) {
            const observer = new MutationObserver(updateMobileTotalDisplay);
            observer.observe(totalAmount, { childList: true, characterData: true, subtree: true });
            updateMobileTotalDisplay(); // Actualizar inicial
        }
    });
} else {
    const totalAmount = document.querySelector('.total-amount');
    if (totalAmount) {
        const observer = new MutationObserver(updateMobileTotalDisplay);
        observer.observe(totalAmount, { childList: true, characterData: true, subtree: true });
        updateMobileTotalDisplay(); // Actualizar inicial
    }
}
