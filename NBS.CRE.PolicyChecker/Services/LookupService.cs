using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace NBS.CRE.PolicyChecker.Services
{
    public class LookupService : ILookupService
    {
        private List<string> _hashList = new List<string>();

        public LookupService()
        {
            _hashList.Add("E92B7B8638A08826DFD322FDDA6CB95E");
            _hashList.Add("9C70933719F4924D1E9C96BB0C0B72EC");
            _hashList.Add("74224CBE9133B6023129C020C4B0DC74");
            _hashList.Add("A119BD778A7E7E16C4B6C7E881A9779E");
            _hashList.Add("185EEC7683F2DB1665FB169916FE8F6D");
            _hashList.Add("D0FEA77C096091F02524E5948890CA35");
            _hashList.Add("AAD6F4820F81E81249E0E85C117BDC02");
            _hashList.Add("0958BAFDFFB98AE98E2F7872F9097D68");
            _hashList.Add("EC6E3585C53CC6EE9C57B21DB19EECA2");
            _hashList.Add("5D7D18A4EDCFE0AF18D69C926C4BCCAA");
            _hashList.Add("2DEA9EB8BB2F83ADDDF5D2AB4F051C67");
            _hashList.Add("50F7907690819717AFC09FC156BDA3DA");
            _hashList.Add("11ABF87B13F0CE7CF604409971CFE4A6");
            _hashList.Add("03F3D2B0CF56CB8CB88FB24F75569C25");
            _hashList.Add("FB20F5770D77AB9073C5C3CEDC2BC0E4");
            _hashList.Add("D182FED8BD9F97C9F020E33B9D3125EF");
            _hashList.Add("5DD37D650B15DDE0477233CE8852B5D6");
            _hashList.Add("37816054A69A8E91B214F9B2EB3F4F2E");
            _hashList.Add("B90509E0CD4953649453F099B6006A0B");
            _hashList.Add("CFB7FD1826C5F6876F7CE33B48C7BE78");
            _hashList.Add("4A6C697A143BEB4435A9879D46D60613");
            _hashList.Add("67708564D078B07545621745E02EC1A4");
            _hashList.Add("685A320F5A140568EC41707667031248");
            _hashList.Add("B0E76CBA536A02A784578C9FE89EF44B");
            _hashList.Add("2DAC65F2E431BFB087BB96723D0EC5F2");
            _hashList.Add("5EC3A64DF84744D281B69C8CD4A1F916");
            _hashList.Add("7051B1D03601A4F13E5E478017D27273");
            _hashList.Add("BCAD96BCDD63B8014F2854D50A11624E");
            _hashList.Add("94CA44F7AD7D662A14FDD16DEE7631AD");
            _hashList.Add("94949F6C0FE3A6EFE5DA477608AFA244");
            _hashList.Add("638DA71525AED8C80A8F259A0EB72136");
            _hashList.Add("AB4D240656F3EF0CF8382CA62AEA9CFD");
            _hashList.Add("7BCD2A838A74C06D3704B9FF8E3190BD");
            _hashList.Add("89207FC27CAB2A6972AA01BFEC1A84AA");
            _hashList.Add("1E25F8F2BDEA742F665C0C4B21276FA5");
            _hashList.Add("02D31E16EB269C24E5BFED676DE1D896");
            _hashList.Add("F2C4A1784648D350A6B01817DD1D7D0E");
            _hashList.Add("32DBBCED2D9FF6AC44E3B6F34C768E3B");
            _hashList.Add("74727E0700EDFD887EF90FF0E7CB11B0");
            _hashList.Add("7F816DD32289327948104732DEC4E210");
            _hashList.Add("C16B8AF5524A2986B4EBE4F2D834835D");
            _hashList.Add("0B792D76CB63AC0B53F7AA8E9840AA4A");
            _hashList.Add("3C6B3EB67C70F8AF68784380EB1BBE72");
            _hashList.Add("32E33DEE41288E4CFB7E8551245347D2");
            _hashList.Add("7A7DA4E4ACAE2D25868C09E87CCEF509");
            _hashList.Add("FA4B24145529ECCD26931CAF640541E5");
            _hashList.Add("0210FB5AAD6C17BC42AC55CAB700324B");
            _hashList.Add("A150B4ABD218A8EE335994F9FB6656BC");
            _hashList.Add("AB49AA96F2F6B9E9858F999965CFB2CC");
            _hashList.Add("ED53ED4C61E3758D2BCED680930970BA");
            _hashList.Add("B05DD1AD8F52C9D157656F599AFA67C3");
            _hashList.Add("F9ACD77BF4A4D64B40314A19A8B4F1D5");
            _hashList.Add("AF2E3CD36ED2573458875F42DCC83509");
            _hashList.Add("A4FA06D3CA8B2DCAE10EAE8031815749");
            _hashList.Add("A3240448F09DB3A684204BF25DC46674");
            _hashList.Add("BBDE0873A7E87F2A2CF931CC0600D4DC");
            _hashList.Add("90236C94FD9BEF40B805FA30CEE5258D");
            _hashList.Add("6E46B0F70A41BEC0B7F4B6D3F61194DE");
            _hashList.Add("33C9C35A0D55D709A2AE584A1BCF8749");
            _hashList.Add("6B7D9D4C2E73E4948490CBCD7F67557D");
            _hashList.Add("5C667FE209C7BC3FB1C94499BF8A1D70");
            _hashList.Add("CA8C81D19B658A6F0F65D43FFFA45EC2");
            _hashList.Add("E8087D2BF8686EBAC9D4AB9C724F092A");
            _hashList.Add("5E2F366B86F8DB35EDD36BC9636D92A7");
            _hashList.Add("BC36B67098DA838958826C5DAEA1A80E");
            _hashList.Add("0B627D9586D08C29339215308764D195");
            _hashList.Add("C384F8ACC0E27552CCA98A022D27FDED");
            _hashList.Add("34D25F4966F0BFE379BBB9E05C8FB3FA");
            _hashList.Add("7AA784E650AFC52D9086C694CF89436B");
            _hashList.Add("10A9884BBCA5E39D0D820CCF7DEF991F");
            _hashList.Add("BEE484D18FC701981AFFC2FEAC1F2BCA");
            _hashList.Add("F2E1F6AFE8E75968CFF6663A58F44775");
            _hashList.Add("A7719E25D52CB119F2255D721E5F8170");
            _hashList.Add("F4EF43A236B0E3DCB9449F92C6995670");
            _hashList.Add("1431D7F0B26079E6230FD6ADD8A15EAE");
            _hashList.Add("EA07A1E71D16A129093A33F80908BDE8");
            _hashList.Add("E0ADC107D007CF17412F8DFA936BDCD5");
            _hashList.Add("D43262AEE423B0DC4EC73178878FF2F0");
            _hashList.Add("90FFFCF2A435A44A9F6BF55BE03D9F29");
            _hashList.Add("567D7F2F80E27DE22687E85ACAFB3077");
            _hashList.Add("44D5D6EE87111FEEBCAEF56451247C01");
            _hashList.Add("BAD8B00AC4C7B09F8CB80266F54F143A");
            _hashList.Add("14E18B8BE6D880F924E44A4C347ADC83");
            _hashList.Add("EF27881B9F577CA95D21D880F4F24E4A");
            _hashList.Add("367C8029DAEF54444B55BEAA86C01061");
            _hashList.Add("44298423AAB7B6E330280956B9ACDDB7");
            _hashList.Add("43F8295F5B20487B0252A28D15FD145E");
            _hashList.Add("AB2E1135E1F412ACEA648B819B96BD7B");
            _hashList.Add("AE7DE7AF8459AA0CAAC218913BF9F95D");
            _hashList.Add("F99471A6AA24EC6A09060E4C8937CF83");
            _hashList.Add("8847F32944CB223569CE626CA3967516");
            _hashList.Add("0010055DB162965D6038A4D13E7FFE5A");
            _hashList.Add("24C61529AC2F1AB613DFA8C8268EE7EB");
            _hashList.Add("5AEC28F1DD915188B8CE51F99891B753");
            _hashList.Add("78DC367E9BFD3D2B9CEE9CC0D61C81BC");
            _hashList.Add("2E42FFB452F7DFFE92AB636CE656A4F8");
            _hashList.Add("33E47223FF124FED7DEE2E7299255A32");
            _hashList.Add("E8D490D6B0BE071DB04A7501F66D83C9");
            _hashList.Add("368723BCFF7F99919B558CAF1D42E7FD");
            _hashList.Add("B040D5D0E1F3AACC76857283561DFDAC");
        }
        public bool CustomerExists(string firstName, string lastName, DateTime dob, string postcode)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(firstName.Trim().ToUpper());
            sb.Append(lastName.Trim().ToUpper());
            sb.Append(dob.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
            sb.Append(postcode.Trim().ToUpper());

            string hash = CalculateMD5Hash(sb.ToString());
            return _hashList.Contains(hash);
        }
        private string CalculateMD5Hash(string input)
        {
            MD5 md5 = MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
