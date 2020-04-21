using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTask.Model
{

    #region goodscreate
    //http://open.wdgj.com/OpenApiDoc/ApiInfo.html?OpenAPIID=70031&dictID=89
    public class WdgjGoodsModel
    {
        public GoodsDatalist datalist { get; set; }
    }
    public class GoodsDatalist
    {
        public List<GoodsDatainfo> datainfo { get; set; }
    }

    public class GoodsDatainfo
    {
        public string goodsid { get; set; }
        public string goodsno { get; set; }
        public string classid { get; set; }
        public string classname { get; set; }
        public string goodsname { get; set; }
        public string goodsname2 { get; set; }
        public string engname { get; set; }
        public string unit { get; set; }
        public string pricedetail { get; set; }
        public string pricemember { get; set; }
        public string price1 { get; set; }
        public string price2 { get; set; }
        public string price3 { get; set; }
        public string bonuspoints { get; set; }
        public string barcode { get; set; }
        public string hscode { get; set; }
        public string brand { get; set; }
        public string origin { get; set; }
        public string weight { get; set; }
        public string pycode { get; set; }
        public int bgift { get; set; }
        public string bblockup { get; set; }
        public string reserved1 { get; set; }
        public string reserved2 { get; set; }
        public string reserved3 { get; set; }
        public string reserved4 { get; set; }
        public string remark { get; set; }
        public string bmultispec { get; set; }
        public string goodslen { get; set; }
        public string goodswidth { get; set; }
        public string goodsheight { get; set; }
        public string days { get; set; }
        public string purchaser { get; set; }
        public string pricebottom { get; set; }
        public string pricewholesale { get; set; }
        public string commisionpara { get; set; }
        public string commisiontype { get; set; }
        public string bnegativestock { get; set; }
        public string bpresell { get; set; }
        public string selldate { get; set; }
        public string bagent { get; set; }
        public string bpackage { get; set; }
        public string bnobyair { get; set; }
        public string blarge { get; set; }
        public string bsn { get; set; }
        public string newtime { get; set; }
        public string bprocess { get; set; }
        public List<specinfo> speclist { get; set; }
        public string goodsspec { get; set; }
    }


    public class specinfo
    {
        public string specid { get; set; }
        public string speccode { get; set; }
        public string specname { get; set; }
        public string fixcostprice { get; set; }
        public string bfixcost { get; set; }
        public string weight { get; set; }
        public string bblockup { get; set; }
        public string remark { get; set; }
        public string expdate { get; set; }
        public List<Warehouseinfo> warehouselist { get; set; }
    }


    public class Warehouseinfo
    {
        public string warehouseno { get; set; }
        public string warehouseid { get; set; }
        public string warehousename { get; set; }
        public string warehousetype { get; set; }
        public string bblockup { get; set; }
        public string country { get; set; }
        public string province { get; set; }
        public string city { get; set; }
        public string town { get; set; }
        public string adr { get; set; }
        public string linkman { get; set; }
        public string zip { get; set; }
        public string tel { get; set; }
        public string bnegativestock { get; set; }
        public string bautoaddgoods { get; set; }
    }


    public class WdgjGoodsResponse
    {
        public string returncode { get; set; }

        public string eturninfo { get; set; }
        public List<GoodsDatainfo> datalist { get; set; }
    }
    #endregion



    public class WdgjSupplierModel
    {
        public SupplierDatalist datalist { get; set; }
    }
    public class SupplierDatalist
    {
        public List<SupplierDatainfo> datainfo { get; set; }
    }

    public class SupplierDatainfo
    {
        public string providerid { get; set; }
        public string providername { get; set; }
        public string providerno { get; set; }
        public string linkman { get; set; }
        public string tel { get; set; }
        public string qq { get; set; }
        public string fax { get; set; }
        public string adr { get; set; }
        public string zip { get; set; }
        public string im { get; set; }
        public string email { get; set; }
        public string website { get; set; }
        public string remark { get; set; }
        public string products { get; set; }
        public string bblockup { get; set; }
        public string pycode { get; set; }
        public string esno { get; set; }
        public string power { get; set; }
        public string powerrate { get; set; }
        public string provideralias { get; set; }
        public string country { get; set; }
        public string province { get; set; }
        public string city { get; set; }
        public string town { get; set; }
        public string bankacct { get; set; }
        public string depositmoney { get; set; }
        public string bdepositsell { get; set; }
    }


    public class WdgjSupplierResponse
    {
        public string returncode { get; set; }

        public string eturninfo { get; set; }
        public List<SupplierDatainfo> datalist { get; set; }
    }
}
