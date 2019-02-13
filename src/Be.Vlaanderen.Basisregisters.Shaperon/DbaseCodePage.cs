namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Text;

    public class DbaseCodePage : IEquatable<DbaseCodePage>
    {
        public static readonly DbaseCodePage None = new DbaseCodePage(0x0, Encoding.UTF8.CodePage);
        public static readonly DbaseCodePage USA_MSDOS = new DbaseCodePage(0x1, 437);
        public static readonly DbaseCodePage Multilingual_MSDOS = new DbaseCodePage(0x2, 850);

        public static readonly DbaseCodePage System_Windows_ANSI_Alias1 =
            new DbaseCodePage(0x3, Encoding.Default.CodePage);

        public static readonly DbaseCodePage Standard_Macintosh = new DbaseCodePage(0x4);
        public static readonly DbaseCodePage Danish_OEM = new DbaseCodePage(0x08, 865);
        public static readonly DbaseCodePage Dutch_OEM_Alias1 = new DbaseCodePage(0x09, 437);
        public static readonly DbaseCodePage Dutch_OEM_Alias2 = new DbaseCodePage(0x0A, 850);
        public static readonly DbaseCodePage Finnish_OEM = new DbaseCodePage(0x0B, 437);
        public static readonly DbaseCodePage French_OEM_Alias1 = new DbaseCodePage(0x0D, 437);
        public static readonly DbaseCodePage French_OEM_Alias2 = new DbaseCodePage(0x0E, 850);
        public static readonly DbaseCodePage German_OEM_Alias1 = new DbaseCodePage(0x0F, 437);
        public static readonly DbaseCodePage German_OEM_Alias2 = new DbaseCodePage(0x10, 850);
        public static readonly DbaseCodePage Italian_OEM_Alias1 = new DbaseCodePage(0x11, 437);
        public static readonly DbaseCodePage Italian_OEM_Alias2 = new DbaseCodePage(0x12, 850);
        public static readonly DbaseCodePage Japanese_Shift_JIS_Alias1 = new DbaseCodePage(0x13, 932);
        public static readonly DbaseCodePage Spanish_OEM_Alias1 = new DbaseCodePage(0x14, 850);
        public static readonly DbaseCodePage Swedish_OEM_Alias1 = new DbaseCodePage(0x15, 437);
        public static readonly DbaseCodePage Swedish_OEM_Alias2 = new DbaseCodePage(0x16, 850);
        public static readonly DbaseCodePage Norwegian_OEM = new DbaseCodePage(0x17, 865);
        public static readonly DbaseCodePage Spanish_OEM_Alias2 = new DbaseCodePage(0x18, 437);
        public static readonly DbaseCodePage BritishEnglish_OEM_Alias1 = new DbaseCodePage(0x19, 437);
        public static readonly DbaseCodePage BritishEnglish_OEM_Alias2 = new DbaseCodePage(0x1A, 850);
        public static readonly DbaseCodePage USEnglish_OEM = new DbaseCodePage(0x1B, 437);
        public static readonly DbaseCodePage CanadianFrench_OEM = new DbaseCodePage(0x1C, 863);
        public static readonly DbaseCodePage French_OEM_Alias3 = new DbaseCodePage(0x1D, 850);
        public static readonly DbaseCodePage Czech_OEM = new DbaseCodePage(0x1F, 852);
        public static readonly DbaseCodePage Hungarian_OEM = new DbaseCodePage(0x22, 852);
        public static readonly DbaseCodePage Polish_OEM = new DbaseCodePage(0x23, 852);
        public static readonly DbaseCodePage Portuguese_OEM_Alias1 = new DbaseCodePage(0x24, 860);
        public static readonly DbaseCodePage Portuguese_OEM_Alias2 = new DbaseCodePage(0x25, 850);
        public static readonly DbaseCodePage Russian_OEM = new DbaseCodePage(0x26, 866);
        public static readonly DbaseCodePage USEnglish_OEM_Alias2 = new DbaseCodePage(0x37, 850);
        public static readonly DbaseCodePage Romanian_OEM = new DbaseCodePage(0x40, 852);
        public static readonly DbaseCodePage Chinese_GBK_PRC = new DbaseCodePage(0x4D, 936);
        public static readonly DbaseCodePage Korean_ANSI_OEM = new DbaseCodePage(0x4E, 949);
        public static readonly DbaseCodePage Chinese_Big5 = new DbaseCodePage(0x4F, 950);
        public static readonly DbaseCodePage Thai_ANSI_OEM = new DbaseCodePage(0x50, 874);

        public static readonly DbaseCodePage System_Windows_ANSI_Alias2 =
            new DbaseCodePage(0x57, Encoding.Default.CodePage);

        public static readonly DbaseCodePage Western_European_ANSI = new DbaseCodePage(0x58, 1252);
        public static readonly DbaseCodePage Spanish_ANSI = new DbaseCodePage(0x59, 1252);
        public static readonly DbaseCodePage Eastern_European_MSDOS = new DbaseCodePage(0x64, 852);
        public static readonly DbaseCodePage Russian_MSDOS = new DbaseCodePage(0x65, 866);
        public static readonly DbaseCodePage Nordic_MSDOS = new DbaseCodePage(0x66, 865);
        public static readonly DbaseCodePage Icelandic_MSDOS = new DbaseCodePage(0x67, 861);
        public static readonly DbaseCodePage Greek_MSDOS = new DbaseCodePage(0x6A, 737);
        public static readonly DbaseCodePage Turkish_MSDOS = new DbaseCodePage(0x6B, 857);
        public static readonly DbaseCodePage CanadianFrench_MSDOS = new DbaseCodePage(0x6C, 863);
        public static readonly DbaseCodePage Taiwan_Big5 = new DbaseCodePage(0x78, 950);
        public static readonly DbaseCodePage Hangul_Wansung = new DbaseCodePage(0x79, 949);
        public static readonly DbaseCodePage PRC_GBK = new DbaseCodePage(0x7A, 936);
        public static readonly DbaseCodePage Japanese_Shift_JIS_Alias2 = new DbaseCodePage(0x7B, 932);
        public static readonly DbaseCodePage Thai_Windows_MSDOS = new DbaseCodePage(0x7C, 874);
        public static readonly DbaseCodePage Greek_OEM = new DbaseCodePage(0x86, 737);
        public static readonly DbaseCodePage Slovenian_OEM = new DbaseCodePage(0x87, 852);
        public static readonly DbaseCodePage Turkish_OEM = new DbaseCodePage(0x88, 857);
        public static readonly DbaseCodePage Eastern_European_Windows = new DbaseCodePage(0xC8, 1250);
        public static readonly DbaseCodePage Russian_Windows = new DbaseCodePage(0xC9, 1251);
        public static readonly DbaseCodePage Turkish_Windows = new DbaseCodePage(0xCA, 1254);
        public static readonly DbaseCodePage Greek_Windows = new DbaseCodePage(0xCB, 1253);
        public static readonly DbaseCodePage Baltic_Windows = new DbaseCodePage(0xCC, 1257);

        public static readonly DbaseCodePage[] All =
        {
            None,
            USA_MSDOS,
            Multilingual_MSDOS,
            System_Windows_ANSI_Alias1,
            Standard_Macintosh,
            Danish_OEM,
            Dutch_OEM_Alias1,
            Dutch_OEM_Alias2,
            Finnish_OEM,
            French_OEM_Alias1,
            French_OEM_Alias2,
            German_OEM_Alias1,
            German_OEM_Alias2,
            Italian_OEM_Alias1,
            Italian_OEM_Alias2,
            Japanese_Shift_JIS_Alias1,
            Spanish_OEM_Alias1,
            Swedish_OEM_Alias1,
            Swedish_OEM_Alias2,
            Norwegian_OEM,
            Spanish_OEM_Alias2,
            BritishEnglish_OEM_Alias1,
            BritishEnglish_OEM_Alias2,
            USEnglish_OEM,
            CanadianFrench_OEM,
            French_OEM_Alias3,
            Czech_OEM,
            Hungarian_OEM,
            Polish_OEM,
            Portuguese_OEM_Alias1,
            Portuguese_OEM_Alias2,
            Russian_OEM,
            USEnglish_OEM_Alias2,
            Romanian_OEM,
            Chinese_GBK_PRC,
            Korean_ANSI_OEM,
            Chinese_Big5,
            Thai_ANSI_OEM,
            System_Windows_ANSI_Alias2,
            Western_European_ANSI,
            Spanish_ANSI,
            Eastern_European_MSDOS,
            Russian_MSDOS,
            Nordic_MSDOS,
            Icelandic_MSDOS,
            Greek_MSDOS,
            Turkish_MSDOS,
            CanadianFrench_MSDOS,
            Taiwan_Big5,
            Hangul_Wansung,
            PRC_GBK,
            Japanese_Shift_JIS_Alias2,
            Thai_Windows_MSDOS,
            Greek_OEM,
            Slovenian_OEM,
            Turkish_OEM,
            Eastern_European_Windows,
            Russian_Windows,
            Turkish_Windows,
            Greek_Windows,
            Baltic_Windows
        };

        public static readonly DbaseCodePage[] AllWithCodePage =
            Array.FindAll(All, item => item._codePage.HasValue);

        private readonly byte _value;
        private readonly int? _codePage;

        private DbaseCodePage(byte value, int? codePage = default)
        {
            _value = value;
            _codePage = codePage;
        }

        public static DbaseCodePage Parse(byte value)
        {
            if (!Array.Exists(All, candidate => candidate._value == value))
                throw new ArgumentException($"The dbase code page {value} is not supported.", nameof(value));

            return Array.Find(All, candidate => candidate._value == value);
        }

        public static bool TryParse(byte value, out DbaseCodePage parsed)
        {
            parsed = Array.Exists(All, candidate => candidate._value == value)
                ? Array.Find(All, candidate => candidate._value == value)
                : null;

            return parsed != null;
        }

        public Encoding ToEncoding(EncodingProvider provider = default)
        {
            if (provider == null)
            {
                return _codePage.HasValue ? Encoding.GetEncoding(_codePage.Value) : null;
            }
            return _codePage.HasValue ? provider.GetEncoding(_codePage.Value) : null;
        }

        public bool Equals(DbaseCodePage other) => other != null && other._value == _value;
        public override bool Equals(object obj) => obj is DbaseCodePage other && Equals(other);
        public override int GetHashCode() => _value;
        public override string ToString() => _value.ToString();

        public byte ToByte() => _value;

        //public static implicit operator byte(DbaseCodePage instance) => instance.ToByte();
    }
}
