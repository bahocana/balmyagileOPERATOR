using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalmyAgilev1
{
    using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

    public class ApiRequestModel
    {
        public string SessionId { get; set; }
        public string RequestBody { get; private set; }
        public string Url { get; private set; }

        public ApiRequestModel(string sessionId)
        {
            SessionId = sessionId;
        }

        public string SiparisListele()
        {
            Url = "https://balmy.ws.dia.com.tr/api/v3/scf/json";
            RequestBody = $@"
        {{
    ""scf_siparis_listele"": {{
        ""session_id"": ""{SessionId}"",
        ""firma_kodu"": 2,
        ""donem_kodu"": 8,
        ""filters"": [
            {{
                ""field"": ""siparisdurum"",
                ""operator"": ""="",
                ""value"": ""+ Bekleyen Miktarlar Var""
            }}
        ],
        ""sorts"": """",
        ""params"": """",
        ""limit"": 0,
        ""offset"": 0
    }}
}}";
            return RequestBody;
        }

        //+ Bekleyen Miktarlar Var
        //siparisdurum

        public string SiparisListeledetail()
        {
            Url = "https://balmy.ws.dia.com.tr/api/v3/scf/json";
            RequestBody = $@"
        {{
            ""scf_siparis_listele_ayrintili"":{{
                ""session_id"": ""{SessionId}"",
                ""firma_kodu"": 2,
                ""donem_kodu"": 9,
         ""filters"": [
            {{
                ""field"": ""siparisdurum"",
                ""operator"": ""="",
                ""value"": ""+ Bekleyen Miktarlar Var""
            }},
            {{
                ""field"": ""carigrupkodu"",
                ""operator"": ""="",
                ""value"": ""MÜŞTERİLER""
            }}
        ],
                ""sorts"": """",
  ""params"": {{
            ""selectedcolumns"": [
                ""unvan"",
                ""fisno"",
                ""siptarih"",
                ""siparisdurum"",
                 ""kartkodu"",
                ""kartaciklama"",
                ""teslimattarihi"",
                ""kartekalan1"",
                ""miktar"",
                ""bekleyenmiktar"",
                ""__dk__3""

            ]
        }},
                ""limit"": 10,
                ""offset"": 0
            }}
        }}";
            return RequestBody;
        }


        // siptarih
        // unvan
        //fisno
        //siparisdurum
        //sipbirimi
        //kartaciklama
        //teslimattarihi
        //kartozelkodu2aciklama
        //kartekalan1
        //kartkodu
        //turuack = Alınan
        //teslimedilmeyenmiktar
        //miktar
        //bekleyenmiktar
        //__dk__3


        public string FaturaListele()
        {
            Url = "https://balmy.ws.dia.com.tr/api/v3/scf/json";
            RequestBody = $@"
        {{
            ""scf_fatura_listele"": {{
                ""session_id"": ""{SessionId}"",
                ""firma_kodu"": 2,
                ""donem_kodu"": 8,
                ""filters"": """",
                ""sorts"": """",
                ""params"": """",
                ""limit"": 10000,
                ""offset"": 0
            }}
        }}";
            return RequestBody;
        }

        public string FaturaListeledetail()
        {
            Url = "https://balmy.ws.dia.com.tr/api/v3/scf/json";
            RequestBody = $@"
        {{
            ""scf_fatura_listele_ayrintili"": {{
                ""session_id"": ""{SessionId}"",
                ""firma_kodu"": 2,
                ""donem_kodu"": 9,
                ""filters"": [
            {{
                ""field"": ""tarih"",
                ""operator"": "">="",
                ""value"": ""2025-01-01""
            }}
        ],
                ""sorts"": """",
                 ""params"": {{
            ""selectedcolumns"": [
                ""anamiktar"",
                ""dagilimkalanmiktar"",
                ""tarih"",
                ""_key_scf_carikart"",
                 ""tutari"",
                ""toplamtutar"",
                ""kartozelkodu1"",
                ""unvan"",
                ""toplamkdvtutari"",
                ""__dk__3"",
                ""sonbirimfiyati"",
                ""fatbirimi"",
                ""siparisno"",
                ""birimfiyati"",
                ""_key_scf_fatura"",
                ""carikodu"",
                ""kalemdovizi"",
                ""_key_scf_irsaliye_kalemi"",
                ""ortalamavade"",
                ""odemeplani"",
                ""_key_sis_sube_source"",
                ""kalanhizmetmaliyettutari"",
                ""_cdate"",
                ""sonbirimfiyatifisdovizi"",
                ""kartkodu"",
                ""toplamkdvharictutar"",
                ""sube"",
                ""siparistarih"",
                ""depo"",
                ""turuack"",
                ""kdvtutarisatirdovizi"",
                ""kdvharictutar"",
                ""kdv"",
                ""toplamtutarsatirdovizi"",
                ""_date"",
                ""yerelbirimfiyati"",
                ""tutarisatirdovizi""
            ]
        }},
                ""limit"": 10,
                ""offset"": 0
            }}
        }}";
            return RequestBody;
        }
        // fatura liste ayrıntılı
        // anamiktar , dagilimkalanmiktar
        //tarih , _key_scf_carikart , tutari , toplamtutar , kartozelkodu1,sontutaryerel,unvan,toplamkdvtutari,__dk__3,sonbirimfiyati,fatbirimi
        // siparisno, birimfiyati,_key_scf_fatura,kartaciklama,carikodu,kalemdovizi,_key_scf_irsaliye_kalemi,ortalamavade,odemeplani,_key_sis_sube_source,

        // kalanhizmetmaliyettutari,sonbirimfiyatifisdovizi,kartkodu,_cdate,toplamkdvharictutar,sube, siparistarih,depo,turuack,kdvtutarisatirdovizi,kdvharictutar,
        //miktar, kdv, toplamtutarsatirdovizi, _date, yerelbirimfiyati, tutarisatirdovizi


        // turuack
        // Alım İade
        // Alınan Hizmet
        // Perakende Satış
        // Toptan Satış
        // Verilen hizmet



        public string bankalist()
        {
            Url = "https://balmy.ws.dia.com.tr/api/v3/sis/json";
            RequestBody = $@"
        {{
          ""sis_banka_listele"": {{
                        ""session_id"": ""{SessionId}"",
                        ""firma_kodu"": 2,
                        ""donem_kodu"": 9,
                        ""filters"": """",
                        ""sorts"": """",
                        ""params"": """",
                        ""limit"": 0,
                        ""offset"": 0
            }}
        }}";
            return RequestBody;
        }









    }

}
