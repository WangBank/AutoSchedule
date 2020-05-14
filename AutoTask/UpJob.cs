using AutoTask.Model;
using BankDbHelper;
using ExcuteInterface;
using Jdwl.Api;
using Newtonsoft.Json;
using OpenApiSDK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace AutoTask
{
    public class UpJob : IUpJob
    {

        public string ExecJob(JobPara jobPara, List<Datas> dsData, out string result)
        {
            string allResult = string.Empty;
            BankDbHelper.SqlHelper sqlHelper = new BankDbHelper.SqlHelper(jobPara.dbType, jobPara.connString);
            List<SqlHelperParameter> lstPara = new List<SqlHelperParameter>();
            result = "0";
            IJdClient client;
            string serverUrl = dsData[0].DataMain.Rows[0].DataRowGetStringValue("serverUrl");
            string accessToken = dsData[0].DataMain.Rows[0].DataRowGetStringValue("accessToken");
            string appKey = dsData[0].DataMain.Rows[0].DataRowGetStringValue("appKey");
            string appSecret = dsData[0].DataMain.Rows[0].DataRowGetStringValue("appSecret");
            string Pin = dsData[0].DataMain.Rows[0].DataRowGetStringValue("Pin");

            string wdgj_appkey = dsData[0].DataMain.Rows[0].DataRowGetStringValue("wdgj_appkey");
            string wdgj_appsecret = dsData[0].DataMain.Rows[0].DataRowGetStringValue("wdgj_appsecret");
            string wdgj_accesstoken = dsData[0].DataMain.Rows[0].DataRowGetStringValue("wdgj_accesstoken");
            string wdgj_apiurl = dsData[0].DataMain.Rows[0].DataRowGetStringValue("wdgj_apiurl");
            string createdate = jobPara.dbType == DBTypeEnum.Oracle.ToString() ? "sysdate" : "getdate()";
            #region base info  sync
            //send supplier info to jdwms
            if (jobPara.jobCode == "10001")
            {
                try
                {
                    if (string.IsNullOrEmpty(serverUrl) || string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(appKey) || string.IsNullOrEmpty(appSecret) || string.IsNullOrEmpty(Pin))
                    {
                        result = "数据源中缺少serverUrl、accessToken、appKey、appSecret、Pin!";
                        return result;

                    }
                    else
                    {
                        client = new DefaultJdClient(serverUrl, accessToken, appKey, appSecret);
                    }


                    for (int i = 0; i < dsData.Count; i++)
                    {
                        var request = new Jdwl.Api.Request.Clps.ClpsSynchronizeSupplierLopRequest
                        {
                            Pin = Pin,
                            SupplierModel = new Jdwl.Api.Domain.Clps.ClpsOpenGwService.SynchroSupplierModel
                            {
                                ActionType = "add",
                                Status = "2",
                                SupplierName = dsData[i].DataMain.Rows[0]["MC"].SqlDataBankToString(),
                                SupplierType = "",
                                ClpsSupplierNo = "",
                                IsvSupplierNo = dsData[i].DataMain.Rows[0]["TJBH"].SqlDataBankToString(),
                                Address = dsData[i].DataMain.Rows[0]["ADDR"].SqlDataBankToString(),
                                City = "",
                                Contacts = dsData[i].DataMain.Rows[0]["LXR"].SqlDataBankToString(),
                                County = "",
                                Email = dsData[i].DataMain.Rows[0]["EMAIL"].SqlDataBankToString(),
                                Ext1 = "",
                                Ext2 = "",
                                Ext3 = "",
                                Ext4 = "",
                                Ext5 = "",
                                Fax = "",
                                Mobile = dsData[i].DataMain.Rows[0]["TEL"].SqlDataBankToString(),
                                OwnerName = dsData[i].DataMain.Rows[0]["OwnerName"].SqlDataBankToString(),
                                OwnerNo = dsData[i].DataMain.Rows[0]["ownerCode"].SqlDataBankToString(),
                                Phone = dsData[i].DataMain.Rows[0]["TEL"].SqlDataBankToString(),
                                Province = "",
                                Town = ""
                            }
                        };

                        //{"response":{"content":{"clpsSupplierNo":"CMS4418046522447","code":"1","flag":"success","isvSupplierNo":"00002","message":"成功"}, "code":0}}
                        var response = client.Execute(request);
                        // CommonHelper.Log($"入参:\r\n{JsonConvert.SerializeObject(request)}\r\n 返回数据:\r\n:{JsonConvert.SerializeObject(response)}", "推送供应商信息");

                        //如果请求执行正确,从这里获取强类型返回值
                        SupplierResponseBody returnValue = JsonConvert.DeserializeObject<SupplierResponseBody>(response.Body);
                        if (returnValue.response.code == 0)
                        {

                            //将返回的ClpsSupplierNo保存一下
                            var insertresult = sqlHelper.ExecSql($"insert into GL_SUPER_XH_JDWMS(CLPSSUPPLIERNO,TJBH,SYNCDATE) values('{returnValue.response.content.clpsSupplierNo}','{dsData[i].DataMain.Rows[0]["TJBH"].SqlDataBankToString()}',{createdate})");
                            if (string.IsNullOrEmpty(insertresult))
                            {
                                allResult += $"\r\n推送供应商信息{dsData[i].DataMain.Rows[0]["TJBH"].SqlDataBankToString()}成功";
                            }
                            else
                            {
                                sqlHelper.ExecSql($"insert into ERRORDATA_XH_JDWMS(GUID,ERRORNO,ERRORTYPE,MAINFIELD,ERRORDATE,ERRORMSG) values('{Guid.NewGuid():N}',{dsData[i].DataMain.Rows[0]["TJBH"].SqlDataBankToString()},'gys','TJBH',{createdate},'{returnValue.response.content.message}')");
                                allResult += $"\r\n推送供应商信息{dsData[i].DataMain.Rows[0]["TJBH"].SqlDataBankToString()}失败,原因:{returnValue.response.content.message + insertresult}";
                            }

                        }
                        //响应的原始报文,如果请求失败,从这里获取错误消息代码
                        else
                        {
                            allResult += $"\r\n推送供应商信息失败，入参:{JsonConvert.SerializeObject(request)},\r\n返回:{JsonConvert.SerializeObject(response)}";
                        }

                    }
                    return allResult;
                }
                catch (Exception ex)
                {

                    result = "推送供应商信息失败" + ex.Message;
                    return result;
                }
                finally
                {
                    sqlHelper.Dispose();
                }

            }

            //send drug info to jdwms
            if (jobPara.jobCode == "10002")
            {
                try
                {


                    if (string.IsNullOrEmpty(serverUrl) || string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(appKey) || string.IsNullOrEmpty(appSecret) || string.IsNullOrEmpty(Pin))
                    {
                        result = "数据源中缺少serverUrl、accessToken、appKey、appSecret、Pin!";
                        return result;

                    }
                    else
                    {
                        client = new DefaultJdClient(serverUrl, accessToken, appKey, appSecret);
                    }
                    for (int i = 0; i < dsData.Count; i++)
                    {
                        var OutPackUoms = new List<Jdwl.Api.Domain.Clps.ClpsOpenGwService.OutPackUom>();
                        OutPackUoms.Add(new Jdwl.Api.Domain.Clps.ClpsOpenGwService.OutPackUom
                        {
                            Height = "",
                            Length = "",
                            NetWeight = "",
                            OutUomName = "BigPack",
                            OutUomNo = "LARGEPACKUNIT",
                            OutUomQty = 1,
                            Volume = "",
                            Width = ""
                        });

                        var PackRules = new List<Jdwl.Api.Domain.Clps.ClpsOpenGwService.PackRule>();
                        PackRules.Add(new Jdwl.Api.Domain.Clps.ClpsOpenGwService.PackRule
                        {
                            OutPackUoms = OutPackUoms,
                            PackId = "BigPack",
                            PackName = "BigPack"
                        });

                        var request = new Jdwl.Api.Request.Clps.ClpsTransportSingleItemLopRequest
                        {
                            Pin = Pin,
                            SingleItemRequest = new Jdwl.Api.Domain.Clps.ClpsOpenGwService.SingleItemRequest
                            {
                                ActionType = dsData[i].DataMain.Rows[0]["actionType"].SqlDataBankToString(),
                                WarehouseCode = dsData[i].DataMain.Rows[0]["warehouseNo"].SqlDataBankToString(),
                                OwnerCode = dsData[i].DataMain.Rows[0]["ownerCode"].SqlDataBankToString(),
                                Item = new Jdwl.Api.Domain.Clps.ClpsOpenGwService.Item
                                {
                                    ItemCode = dsData[i].DataMain.Rows[0]["HH"].SqlDataBankToString(),
                                    ItemId = dsData[i].DataMain.Rows[0]["actionType"].SqlDataBankToString() == "add" ? "" : dsData[i].DataMain.Rows[0]["ItemId"].SqlDataBankToString(),
                                    GoodsNumFloat = 0,
                                    ShopNos = dsData[i].DataMain.Rows[0]["shopNos"].SqlDataBankToString(),
                                    SupplierCode = "",
                                    SupplierName = "",
                                    GoodsCode = "",
                                    ClpsGoodsCode = "",
                                    ItemName = dsData[i].DataMain.Rows[0]["PM"].SqlDataBankToString(),
                                    ShortName = dsData[i].DataMain.Rows[0]["BM"].SqlDataBankToString(),
                                    EnglishName = "",
                                    BarCode = dsData[i].DataMain.Rows[0]["TM"].SqlDataBankToString(),
                                    SkuProperty = "",
                                    StockUnitCode = dsData[i].DataMain.Rows[0]["PDW"].SqlDataBankToString(),
                                    StockUnitDesc = dsData[i].DataMain.Rows[0]["PDW"].SqlDataBankToString(),
                                    Length = 0,
                                    Width = 0,
                                    Height = 0,
                                    Volume = 0,
                                    GrossWeight = 0,
                                    NetWeight = 0,
                                    Color = "",
                                    Size = "",
                                    Title = "",
                                    CategoryId = "",
                                    CategoryName = "",
                                    PricingCategory = "",
                                    SafetyStock = "",
                                    ItemType = "ZC",
                                    TagPrice = 0,
                                    RetailPrice = 0,
                                    CostPrice = 0,
                                    PurchasePrice = 0,
                                    SeasonCode = "",
                                    SeasonName = "",
                                    BrandCode = "",
                                    BrandName = "",
                                    IsSNMgmt = "N",
                                    ProductDate = "",
                                    ExpireDate = "",
                                    IsShelfLifeMgmt = "N",
                                    ShelfLife = 0,
                                    RejectLifecycle = 0,
                                    LockupLifecycle = 0,
                                    AdventLifecycle = 0,
                                    IsBatchMgmt = "N",
                                    BatchCode = "",
                                    BatchRemark = "",
                                    OriginAddress = dsData[i].DataMain.Rows[0]["CD"].SqlDataBankToString(),
                                    ApprovalNumber = "",
                                    IsFragile = "",
                                    IsHazardous = "",
                                    Remark = "",
                                    CreateTime = "",
                                    UpdateTime = "",
                                    IsValid = "Y",
                                    IsSku = "",
                                    PackageMaterial = "",
                                    SellerGoodsSign = "",
                                    SpGoodsNo = "",
                                    InstoreThreshold = 0,
                                    OutstoreThreshold = 0,
                                    Manufacturer = dsData[i].DataMain.Rows[0]["SCDW"].SqlDataBankToString(),
                                    SizeDefinition = "",
                                    CheapGift = dsData[i].DataMain.Rows[0]["ISGIFT"].SqlDataBankToInt() == 0 ? "N" : "Y",
                                    Quality = "",
                                    Expensive = "",
                                    Luxury = "",
                                    Liquid = "",
                                    Consumables = "",
                                    Abnormal = "",
                                    Imported = "",
                                    Health = "",
                                    Temperature = "",
                                    TemperatureCeil = "",
                                    TemperatureFloor = "",
                                    Humidity = "",
                                    HumidityCeil = "",
                                    HumidityFloor = "",
                                    Movable = "",
                                    Service3g = "",
                                    Sample = "",
                                    Odor = "",
                                    Sex = "",
                                    Precious = "",
                                    MixedBatch = "",
                                    FashionNo = "",
                                    CustomMade = "",
                                    AirMark = "",
                                    LossRate = "",
                                    SellPeriod = "",
                                    IsPMX = "",
                                    QualityCheckRate = "",
                                    ProductSeason = "",
                                    MaterialNo = "",
                                    PrIntegerProductId = "",
                                    PrIntegerName = "",
                                    BundleFlag = "",
                                    ProductCategory = 0,
                                    Category = dsData[i].DataMain.Rows[0]["YPBZ"].SqlDataBankToString() == "1" ? "Y" : "N",
                                    Storage = dsData[i].DataMain.Rows[0]["CCTJ"].SqlDataBankToString() == "" ? "常温" : dsData[i].DataMain.Rows[0]["CCTJ"].SqlDataBankToString(),
                                    Type = "",
                                    Specification = dsData[i].DataMain.Rows[0]["GG"].SqlDataBankToString(),
                                    GenericName = "",
                                    Dosage = "",
                                    UseMethods = "",
                                    PackingUnit = "",
                                    Efficacy = "",
                                    RegistrationNum = "",
                                    ApprovalNum = "",
                                    CuringType = 0,
                                    CuringPeriod = 0,
                                    WarmLayer = "",
                                    QualifyTypes = 0,
                                    DoseType = 0,
                                    PackRules = PackRules,
                                    Serial = 0,
                                    TraceNoCollect = 0

                                }
                            }
                        };

                        var jsonrequest = JsonConvert.SerializeObject(request);
                        var response = client.Execute(request);

                        //{"response":{"content":{"clpsGoodsCode":"CMG4418274978871","code":"1","flag":"success","itemCode":"00000002","message":"商品同步成功"}, "code":0}}
                        //如果请求执行正确,从这里获取强类型返回值
                        SingelResponseBody returnValue = JsonConvert.DeserializeObject<SingelResponseBody>(response.Body);
                        if (returnValue.response.code == 0)
                        {
                            if (dsData[i].DataMain.Rows[0]["actionType"].SqlDataBankToString() == "add")
                            {
                                string insertsql = $"insert into YW_KCK_XH_JDWMS(CLPSGOODSCODE,HH,SYNCDATE) values('{returnValue.response.content.clpsGoodsCode}','{dsData[i].DataMain.Rows[0]["HH"].SqlDataBankToString()}',{createdate})";
                                var insertresult = sqlHelper.ExecSql(insertsql);
                                if (string.IsNullOrEmpty(insertresult))
                                {
                                    allResult += $"\r\n推送商品信息{dsData[i].DataMain.Rows[0]["HH"].SqlDataBankToString()}成功";
                                }
                                else
                                {
                                    sqlHelper.ExecSql($"insert into ERRORDATA_XH_JDWMS(GUID,ERRORNO,ERRORTYPE,MAINFIELD,ERRORDATE,ERRORMSG) values('{Guid.NewGuid():N}',{dsData[i].DataMain.Rows[0]["HH"].SqlDataBankToString()},'sp','HH',{createdate},'{returnValue.response.content.message}')");
                                    allResult += $"\r\n推送商品信息{dsData[i].DataMain.Rows[0]["HH"].SqlDataBankToString()}失败,原因:{returnValue.response.content.message + insertresult}";
                                }
                            }

                        }
                        //响应的原始报文,如果请求失败,从这里获取错误消息代码
                        else
                        {
                            allResult += $"\r\n推送商品信息，入参:{JsonConvert.SerializeObject(request)},\r\n返回:{JsonConvert.SerializeObject(response)}";
                        }

                    }
                    return allResult;
                }
                catch (Exception ex)
                {

                    result = "推送商品信息失败" + ex.Message;
                    return result;
                }
                finally
                {
                    sqlHelper.Dispose();
                }

            }

            //query 商品  info
            if (jobPara.jobCode == "20001")
            {
                try
                {

                    if (string.IsNullOrEmpty(serverUrl) || string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(appKey) || string.IsNullOrEmpty(appSecret) || string.IsNullOrEmpty(Pin))
                    {
                        result = "数据源中缺少serverUrl、accessToken、appKey、appSecret、Pin!";
                        return result;

                    }
                    else
                    {
                        client = new DefaultJdClient(serverUrl, accessToken, appKey, appSecret);
                    }
                    for (int i = 0; i < dsData.Count; i++)
                    {
                        var request = new Jdwl.Api.Request.Clps.ClpsQueryItemLopRequest
                        {
                            Pin = Pin,
                            ItemQueryRequest = new Jdwl.Api.Domain.Clps.ClpsOpenGwService.ItemQueryRequest
                            {
                                Page = 1,
                                PageSize = 10,
                                ItemCode = dsData[i].DataMain.Rows[0]["HH"].SqlDataBankToString(),
                                OwnerCode = dsData[i].DataMain.Rows[0]["ownerCode"].SqlDataBankToString(),
                                QueryType = "2"
                            }
                        };

                        var response = client.Execute(request);
                        //如果请求执行正确,从这里获取强类型返回值
                        SelectGoods returnValue = JsonConvert.DeserializeObject<SelectGoods>(response.Body);
                        if (response.Code == 0)
                        {

                            var insertresult = sqlHelper.ExecSql($"update yw_kck_xh_jdwms set CLPSGOODSCODE ='{returnValue.response.content.items[0].itemId}' where HH ='{dsData[i].DataMain.Rows[0]["HH"].SqlDataBankToString()}'");
                            if (string.IsNullOrEmpty(insertresult))
                            {
                                allResult += $"\r\n查询商品信息{dsData[i].DataMain.Rows[0]["HH"].SqlDataBankToString()}成功";
                            }
                            else
                            {
                                allResult += $"\r\n查询商品信息{dsData[i].DataMain.Rows[0]["HH"].SqlDataBankToString()}失败,原因:{insertresult}";
                            }
                        }
                        //响应的原始报文,如果请求失败,从这里获取错误消息代码
                        else
                        {
                            allResult += $"\r\n查询商品信息，入参:{JsonConvert.SerializeObject(request)},\r\n返回:{JsonConvert.SerializeObject(response)}";
                        }

                    }
                    return allResult;
                }
                catch (Exception ex)
                {

                    result = "查询商品信息" + ex.Message;
                    return result;
                }
                finally
                {
                    sqlHelper.Dispose();
                }

            }



            //send shop info to jdwms
            if (jobPara.jobCode == "10003")
            {

                try
                {


                    if (string.IsNullOrEmpty(serverUrl) || string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(appKey) || string.IsNullOrEmpty(appSecret) || string.IsNullOrEmpty(Pin))
                    {
                        result = "数据源中缺少serverUrl、accessToken、appKey、appSecret、Pin!";
                        return result;

                    }
                    else
                    {
                        client = new DefaultJdClient(serverUrl, accessToken, appKey, appSecret);
                    }
                    for (int i = 0; i < dsData.Count; i++)
                    {
                        var request = new Jdwl.Api.Request.Clps.ClpsSynchronizeShopLopRequest
                        {

                            Pin = Pin,
                            ShopIn = new Jdwl.Api.Domain.Clps.ClpsOpenGwService.SynchroShopModel
                            {
                                ActionType = "add",
                                Status = "1",
                                IsvShopNo = dsData[i].DataMain.Rows[0]["CODE"].SqlDataBankToString(),
                                SpSourceNo = "1",
                                OwnerNo = dsData[i].DataMain.Rows[0]["ownerCode"].SqlDataBankToString(),
                                Type = "",
                                ShopNo = "",
                                SpShopNo = "",
                                ShopName = dsData[i].DataMain.Rows[0]["NAME"].SqlDataBankToString(),
                                Contacts = "",
                                Phone = "",
                                Address = "",
                                Email = "",
                                Fax = "",
                                AfterSaleAddress = dsData[i].DataMain.Rows[0]["ADDR"].SqlDataBankToString(),
                                AfterSaleContacts = "售后联系人",
                                AfterSalePhone = dsData[i].DataMain.Rows[0]["TEL"].SqlDataBankToString(),
                                BdOwnerNo = "",
                                OutstoreRules = "",
                                BizType = ""
                            }
                        };

                        //{"response":{"content":{"code":"1","flag":"success","isvShopNo":"0001","message":"成功","shopNo":"CSP0020000045959"}, "code":0}}
                        var response = client.Execute(request);
                        // CommonHelper.Log($"入参:\r\n{JsonConvert.SerializeObject(request)}\r\n 返回数据:\r\n:{JsonConvert.SerializeObject(response)}", "推送供应商信息");

                        //如果请求执行正确,从这里获取强类型返回值
                        ShopResponseBody returnValue = JsonConvert.DeserializeObject<ShopResponseBody>(response.Body);
                        if (returnValue.response.code == 0)
                        {

                            //将返回的ClpsSupplierNo保存一下
                            var insertresult = sqlHelper.ExecSql($"insert into ORGANIZATION_XH_JDWMS(SHOPNO,CODE,SYNCDATE) values('{returnValue.response.content.shopNo}','{dsData[i].DataMain.Rows[0]["CODE"].SqlDataBankToString()}',{createdate})");
                            if (string.IsNullOrEmpty(insertresult))
                            {
                                allResult += $"\r\n推送店铺信息{dsData[i].DataMain.Rows[0]["CODE"].SqlDataBankToString()}成功";
                            }
                            else
                            {
                                sqlHelper.ExecSql($"insert into ERRORDATA_XH_JDWMS(GUID,ERRORNO,ERRORTYPE,MAINFIELD,ERRORDATE,ERRORMSG) values('{Guid.NewGuid():N}',{dsData[i].DataMain.Rows[0]["CODE"].SqlDataBankToString()},'dp','CODE',{createdate},'{returnValue.response.content.message}')");
                                allResult += $"\r\n推送店铺信息{dsData[i].DataMain.Rows[0]["CODE"].SqlDataBankToString()}失败,原因:{returnValue.response.content.message + insertresult}";
                            }
                        }
                        //响应的原始报文,如果请求失败,从这里获取错误消息代码
                        else
                        {
                            allResult += $"\r\n推送店铺信息失败，入参:{JsonConvert.SerializeObject(request)},\r\n返回:{JsonConvert.SerializeObject(response)}";
                        }

                    }
                    return allResult;
                }
                catch (Exception ex)
                {

                    result = "推送店铺信息失败" + ex.Message;
                    return result;
                }
                finally
                {
                    sqlHelper.Dispose();
                }

            }
            #endregion

            //send purchase order info to jdwms
            if (jobPara.jobCode == "10004")
            {

                if (string.IsNullOrEmpty(serverUrl) || string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(appKey) || string.IsNullOrEmpty(appSecret) || string.IsNullOrEmpty(Pin))
                {
                    result = "数据源中缺少serverUrl、accessToken、appKey、appSecret、Pin!";
                    return result;

                }
                else
                {
                    client = new DefaultJdClient(serverUrl, accessToken, appKey, appSecret);
                }
                //dsData[i].DataDetail[0] the i is detail what developer defined
                for (int i = 0; i < dsData.Count; i++)
                {
                    try
                    {
                        var OrderLines = new List<Jdwl.Api.Domain.Clps.ClpsOpenGwService.PoItemModel>();

                        for (int j = 0; j < dsData[i].DataDetail[0].Rows.Count; j++)
                        {
                            OrderLines.Add(new Jdwl.Api.Domain.Clps.ClpsOpenGwService.PoItemModel
                            {
                                OrderLineNo = dsData[i].DataDetail[0].Rows[j]["DETAILNO"].SqlDataBankToString(),
                                ItemNo = dsData[i].DataDetail[0].Rows[j]["CLPSGOODSCODE"].SqlDataBankToString(),
                                PlanQty = dsData[i].DataDetail[0].Rows[j]["QUANTITY"].SqlDataBankToInt(),
                                GoodsStatus = "1"
                            });
                        }

                        var request = new Jdwl.Api.Request.Clps.ClpsAddPoOrderLopRequest
                        {
                            Pin = Pin,
                            PoAddRequest = new Jdwl.Api.Domain.Clps.ClpsOpenGwService.PoAddRequest
                            {
                                EntryOrder = new Jdwl.Api.Domain.Clps.ClpsOpenGwService.PoModel
                                {
                                    EntryOrderCode = dsData[i].DataMain.Rows[0]["BILLNO"].SqlDataBankToString(),
                                    OwnerCode = dsData[i].DataMain.Rows[0]["ownerCode"].SqlDataBankToString(),
                                    //京东那边的供应商编码
                                    SupplierCode = dsData[i].DataMain.Rows[0]["SupplierCode"].SqlDataBankToString(),
                                    WarehouseCode = dsData[i].DataMain.Rows[0]["warehouseNo"].SqlDataBankToString()
                                },
                                OrderLines = OrderLines
                            }
                        };
                        var jsonrequest = JsonConvert.SerializeObject(request);
                        //{"response":{"content":{"code":"1","createTime":"2020-04-03 13:40:51","entryOrderCode":"CPL4418047893011","flag":"success","message":"成功"}, "code":0}}
                        var response = client.Execute(request);

                        //如果请求执行正确,从这里获取强类型返回值
                        OrderResponseBody returnValue = JsonConvert.DeserializeObject<OrderResponseBody>(response.Body);
                        if (returnValue.response.code == 0)
                        {
                            var insertresult = sqlHelper.ExecSql($"insert into STOCKORDERFORM_XH_JDWMS(BILLNO,SYNCDATE,ENTRYORDERCODE) values('{dsData[i].DataMain.Rows[0]["BILLNO"].SqlDataBankToString()}',{createdate},'{returnValue.response.content.entryOrderCode}')");
                            if (string.IsNullOrEmpty(insertresult))
                            {
                                allResult += $"\r\n推送采购订单信息{dsData[i].DataMain.Rows[0]["BILLNO"].SqlDataBankToString()}成功";
                            }
                            else
                            {
                                result = "-1";
                                sqlHelper.ExecSql($"insert into ERRORDATA_XH_JDWMS(GUID,ERRORNO,ERRORTYPE,MAINFIELD,ERRORDATE,ERRORMSG) values('{Guid.NewGuid():N}',{dsData[i].DataMain.Rows[0]["BILLNO"].SqlDataBankToString()},'cgdd','BILLNO',{createdate},'{returnValue.response.content.message}')");
                                allResult += $"\r\n推送采购订单信息{dsData[i].DataMain.Rows[0]["BILLNO"].SqlDataBankToString()}失败,原因:{returnValue.response.content.message + insertresult}";
                            }
                        }
                        //响应的原始报文,如果请求失败,从这里获取错误消息代码
                        else
                        {
                            result = "-1";
                            allResult += $"\r\n推送采购订单信息失败，入参:{JsonConvert.SerializeObject(request)},\r\n返回:{JsonConvert.SerializeObject(response)}";
                        }
                    }
                    catch (Exception ex)
                    {

                        result = "推送采购订单信息失败" + ex.Message;
                        return result;
                    }
                    finally
                    {
                        sqlHelper.Dispose();
                    }

                }
                return allResult;

            }

            //query purchase order info
            if (jobPara.jobCode == "10005")
            {
                try
                {

                    if (string.IsNullOrEmpty(serverUrl) || string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(appKey) || string.IsNullOrEmpty(appSecret) || string.IsNullOrEmpty(Pin))
                    {
                        result = "数据源中缺少serverUrl、accessToken、appKey、appSecret、Pin!";
                        return result;

                    }
                    else
                    {
                        client = new DefaultJdClient(serverUrl, accessToken, appKey, appSecret);
                    }
                    for (int i = 0; i < dsData.Count; i++)
                    {
                        string billno = dsData[i].DataMain.Rows[0]["BILLNO"].SqlDataBankToString();
                        var request = new Jdwl.Api.Request.Clps.ClpsQueryPoOrderLopRequest
                        {
                            Pin = Pin,
                            PoQueryRequest = new Jdwl.Api.Domain.Clps.ClpsOpenGwService.PoQueryRequest
                            {
                                EntryOrderCode = billno,
                                OwnerCode = dsData[i].DataMain.Rows[0]["ownerCode"].SqlDataBankToString(),
                                WarehouseCode = dsData[i].DataMain.Rows[0]["warehouseNo"].SqlDataBankToString()
                            }
                        };

                        var response = client.Execute(request);
                        //{ "response":{ "content":{ "code":"1","entryOrder":{ "clpsOrderCode":"CPL4418047893011","createTime":"2020-04-03 13:40","createUser":"romensfzl","entryOrderCode":"201709250002","ownerCode":"CBU8816093026319","poOrderStatus":"20","supplierCode":"CMS4418046522447","warehouseCode":"800001801"},"flag":"success","message":"成功","poBoxModels":[],"serialNumberList":[],"totalLines":0}, "code":0}}
                        //如果请求执行正确,从这里获取强类型返回值
                        OrderQueryResponseBody returnValue = JsonConvert.DeserializeObject<OrderQueryResponseBody>(response.Body);
                        if (returnValue.response.code == 0)
                        {

                            var insertresult = sqlHelper.ExecSql($"update STOCKORDERFORM_XH_JDWMS set POORDERSTATUS ={returnValue.response.content.entryOrder.poOrderStatus},STORAGESTATUS={returnValue.response.content.entryOrder.storageStatus} where BILLNO ='{dsData[i].DataMain.Rows[0]["BILLNO"].SqlDataBankToString()}'");
                            if (string.IsNullOrEmpty(insertresult))
                            {
                                allResult += $"\r\n查询采购订单信息{dsData[i].DataMain.Rows[0]["BILLNO"].SqlDataBankToString()}成功";
                            }
                            else
                            {
                                allResult += $"\r\n查询采购订单信息{dsData[i].DataMain.Rows[0]["BILLNO"].SqlDataBankToString()}失败,原因:{insertresult}";
                            }
                        }
                        //响应的原始报文,如果请求失败,从这里获取错误消息代码
                        else
                        {
                            allResult += $"\r\n查询采购订单信息失败，入参:{JsonConvert.SerializeObject(request)},\r\n返回:{JsonConvert.SerializeObject(response)}";
                        }

                    }
                    return allResult;
                }
                catch (Exception ex)
                {

                    result = "查询采购订单信息失败" + ex.Message;
                    return result;
                }
                finally
                {
                    sqlHelper.Dispose();
                }

            }


            // return supplier order
            if (jobPara.jobCode == "10006")
            {
                try
                {


                    if (string.IsNullOrEmpty(serverUrl) || string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(appKey) || string.IsNullOrEmpty(appSecret) || string.IsNullOrEmpty(Pin))
                    {
                        result = "数据源中缺少serverUrl、accessToken、appKey、appSecret、Pin!";
                        return result;

                    }
                    else
                    {
                        client = new DefaultJdClient(serverUrl, accessToken, appKey, appSecret);
                    }
                    //dsData[i].DataDetail[0] 这个第i个是根据数据源中写的
                    for (int i = 0; i < dsData.Count; i++)
                    {
                        var RtsOrderItemList = new List<Jdwl.Api.Domain.Clps.ClpsOpenGwService.RtsOrderItem>();

                        for (int j = 0; j < dsData[i].DataDetail[0].Rows.Count; j++)
                        {
                            RtsOrderItemList.Add(new Jdwl.Api.Domain.Clps.ClpsOpenGwService.RtsOrderItem
                            {

                                ItemId = dsData[i].DataDetail[0].Rows[j]["CLPSGOODSCODE"].SqlDataBankToString(),
                                ItemQty = dsData[i].DataDetail[0].Rows[j]["QUANTITY"].SqlDataBankToInt(),
                            });
                        }

                        var request = new Jdwl.Api.Request.Clps.ClpsAddRtsOrderLopRequest
                        {
                            Pin = Pin,
                            RtsOrderRequest = new Jdwl.Api.Domain.Clps.ClpsOpenGwService.RtsOrderRequest
                            {
                                OwnerCode = dsData[i].DataMain.Rows[0]["ownerCode"].SqlDataBankToString(),
                                OutRtsOrderCode = dsData[i].DataMain.Rows[0]["BILLNO"].SqlDataBankToString(),
                                WarehouseCode = dsData[i].DataMain.Rows[0]["warehouseNo"].SqlDataBankToString(),
                                ExtractionWay = 1,
                                RtsOrderType = 1,
                                SupplierNo = dsData[i].DataMain.Rows[0]["SUPPLIERCODE"].SqlDataBankToString(),
                                ReceiverInfo = new Jdwl.Api.Domain.Clps.ClpsOpenGwService.ReceiverInfo
                                {
                                    Mobile = dsData[i].DataMain.Rows[0]["TEL"].SqlDataBankToString()
                                },
                                //出库单明细
                                RtsOrderItemList = RtsOrderItemList
                            }
                        };
                        var jsonrequest = JsonConvert.SerializeObject(request);
                        //{"response":{"content":{"clpsRtsNo":"CBS4418046753361","code":"1","flag":"success","message":"成功"}, "code":0}}
                        var response = client.Execute(request);

                        //如果请求执行正确,从这里获取强类型返回值
                        RtsOrderResponseBody returnValue = JsonConvert.DeserializeObject<RtsOrderResponseBody>(response.Body);
                        if (returnValue.response.code == 0)
                        {

                            var insertresult = sqlHelper.ExecSql($"insert into STOCKRETURNAPPROVE_XH_JDWMS(BILLNO,SYNCDATE,CLPSRTSNO) values('{ dsData[i].DataMain.Rows[0]["BILLNO"].SqlDataBankToString()}',{createdate},'{returnValue.response.content.clpsRtsNo}')");
                            if (string.IsNullOrEmpty(insertresult))
                            {
                                allResult += $"\r\n推送退供应商订单{dsData[i].DataMain.Rows[0]["BILLNO"].SqlDataBankToString()}成功";
                            }
                            else
                            {
                                sqlHelper.ExecSql($"insert into ERRORDATA_XH_JDWMS(GUID,ERRORNO,ERRORTYPE,MAINFIELD,ERRORDATE,ERRORMSG) values('{Guid.NewGuid():N}',{dsData[i].DataMain.Rows[0]["BILLNO"].SqlDataBankToString()},'cgtd','CODE',{createdate},'{returnValue.response.content.message}')");
                                allResult += $"\r\n推送退供应商订单{dsData[i].DataMain.Rows[0]["BILLNO"].SqlDataBankToString()}失败,原因:{returnValue.response.content.message + insertresult}";
                            }
                        }
                        //响应的原始报文,如果请求失败,从这里获取错误消息代码
                        else
                        {
                            allResult += $"\r\n推送退供应商订单失败，入参:{JsonConvert.SerializeObject(request)},\r\n返回:{JsonConvert.SerializeObject(response)}";
                        }

                    }
                    return allResult;
                }
                catch (Exception ex)
                {

                    result = "推送退供应商订单失败" + ex.Message;
                    return result;
                }
                finally
                {
                    sqlHelper.Dispose();
                }

            }

            // query return supplier order info   
            if (jobPara.jobCode == "10007")
            {
                try
                {

                    if (string.IsNullOrEmpty(serverUrl) || string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(appKey) || string.IsNullOrEmpty(appSecret) || string.IsNullOrEmpty(Pin))
                    {
                        result = "数据源中缺少serverUrl、accessToken、appKey、appSecret、Pin!";
                        return result;

                    }
                    else
                    {
                        client = new DefaultJdClient(serverUrl, accessToken, appKey, appSecret);
                    }
                    for (int i = 0; i < dsData.Count; i++)
                    {
                        string billno = dsData[i].DataMain.Rows[0]["CLPSRTSNO"].SqlDataBankToString();
                        var request = new Jdwl.Api.Request.Clps.ClpsIsvRtsQueryLopRequest
                        {
                            Pin = Pin,
                            QueryRtsOrderRequest = new Jdwl.Api.Domain.Clps.ClpsOpenGwService.QueryRtsOrderRequest
                            {
                                RtsOrderNos = billno,
                                OwnerNo = dsData[i].DataMain.Rows[0]["ownerCode"].SqlDataBankToString()
                            }
                        };
                        var jsonrequest = JsonConvert.SerializeObject(request);

                        //{"response":{"content":{"code":"1","flag":"success","message":"成功","rtsResults":[{"deliveryMode":"1","isvRtsCode":"201909090001","operatorTime":"2020-04-07 17:05:31","operatorUser":"romensfzl","ownerNo":"CBU8816093026319","receiverInfo":{"email":"数据为null","mobile":"0578-5082404","name":"宋志强测试供应商"},"rtsCode":"CBS4418046753361","rtsDetailList":[{"goodsStatus":"1","itemId":"CMG4418288906048","itemName":"布洛伪麻胶囊(得尔)","itemNo":"00000007","planOutQty":5.0,"planQty":5}],"rtsOrderStatus":"100","serialNumberList":[],"source":"9","supplierNo":"CMS4418046523757","warehouseNo":"800001573"}],"totalLine":1}, "code":0}}
                        var response = client.Execute(request);

                        //如果请求执行正确,从这里获取强类型返回值
                        RtsOrderQueryResponseBody returnValue = JsonConvert.DeserializeObject<RtsOrderQueryResponseBody>(response.Body);
                        if (returnValue.response.code == 0)
                        {
                            //save rtsorderstatus to STOCKRETURNAPPROVE_XH_JDWMS table
                            var insertresult = sqlHelper.ExecSql($"update STOCKRETURNAPPROVE_XH_JDWMS set RTSORDERSTATUS = '{returnValue.response.content.rtsResults[0].rtsOrderStatus }' where BILLNO ='{dsData[i].DataMain.Rows[0]["BILLNO"].SqlDataBankToString()}'");
                            if (string.IsNullOrEmpty(insertresult))
                            {
                                allResult += $"\r\n查询退供应商订单{dsData[i].DataMain.Rows[0]["BILLNO"].SqlDataBankToString()}成功";
                            }
                            else
                            {
                                allResult += $"\r\n查询退供应商订单{dsData[i].DataMain.Rows[0]["BILLNO"].SqlDataBankToString()}失败,原因:{insertresult}";
                            }
                        }
                        //响应的原始报文,如果请求失败,从这里获取错误消息代码
                        else
                        {
                            allResult += $"\r\n查询退供应商订单失败，入参:{JsonConvert.SerializeObject(request)},\r\n返回:{JsonConvert.SerializeObject(response)}";
                        }

                    }
                    return allResult;
                }
                catch (Exception ex)
                {

                    result = "查询退供应商订单失败" + ex.Message;
                    return result;
                }
                finally
                {
                    sqlHelper.Dispose();
                }

            }

            // query stock   
            if (jobPara.jobCode == "10008")
            {
                try
                {


                    if (string.IsNullOrEmpty(serverUrl) || string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(appKey) || string.IsNullOrEmpty(appSecret) || string.IsNullOrEmpty(Pin))
                    {
                        result = "数据源中缺少serverUrl、accessToken、appKey、appSecret、Pin!";
                        return result;

                    }
                    else
                    {
                        client = new DefaultJdClient(serverUrl, accessToken, appKey, appSecret);
                    }
                    for (int i = 0; i < dsData.Count; i++)
                    {
                        var request = new Jdwl.Api.Request.Clps.ClpsQueryStockLopRequest
                        {
                            Pin = Pin,
                            QueryWarehouseStockRequest = new Jdwl.Api.Domain.Clps.ClpsOpenGwService.QueryWarehouseStockRequest
                            {
                                WarehouseNo = dsData[i].DataMain.Rows[0]["warehouseNo"].SqlDataBankToString(),
                                OwnerNo = dsData[i].DataMain.Rows[0]["ownerCode"].SqlDataBankToString(),
                                GoodsNo = dsData[i].DataMain.Rows[0]["CLPSGOODSCODE"].SqlDataBankToString(),
                                CurrentPage = 1,
                                PageSize = 1
                            }
                        };
                        var jsonrequest = JsonConvert.SerializeObject(request);


                        //{"response":{"content":{"code":"1","flag":"success","message":"成功","totalLines":1,"warehouseStockModelList":[{"goodsName":"耳聋左慈丸","goodsNo":"CMG4418287716460","ownerName":"GXW测试货主勿动","ownerNo":"CBU8816093026319","sellerGoodsSign":"00000001","stockStatus":"1","stockType":"1","totalNum":2000,"totalNumValue":2000.0,"usableNum":2000,"usableNumValue":2000.0,"warehouseName":"GXWj接口测试仓","warehouseNo":"800001801"}]}, "code":0}}
                        var response = client.Execute(request);

                        //如果请求执行正确,从这里获取强类型返回值
                        QueryStockResponseBody returnValue = JsonConvert.DeserializeObject<QueryStockResponseBody>(response.Body);
                        if (returnValue.response.code == 0)
                        {
                            //save rtsorderstatus to STOCKRETURNAPPROVE_XH_JDWMS table
                            var insertresult = sqlHelper.ExecSql($"update YW_KCK_XH_JDWMS set JDWMSSTOCK = '{returnValue.response.content.warehouseStockModelList[0].UsableNum}' where HH ='{dsData[i].DataMain.Rows[0]["HH"].SqlDataBankToString()}'");
                            if (string.IsNullOrEmpty(insertresult))
                            {
                                allResult += $"\r\n查询库存信息{dsData[i].DataMain.Rows[0]["HH"].SqlDataBankToString()}成功";
                            }
                            else
                            {
                                allResult += $"\r\n查询库存信息{dsData[i].DataMain.Rows[0]["HH"].SqlDataBankToString()}失败,原因:{insertresult}";
                            }
                        }
                        //响应的原始报文,如果请求失败,从这里获取错误消息代码
                        else
                        {
                            allResult += $"\r\n查询库存信息，入参:{JsonConvert.SerializeObject(request)},\r\n返回:{JsonConvert.SerializeObject(response)}";
                        }

                    }
                    return allResult;
                }
                catch (Exception ex)
                {

                    result = "查询库存信息" + ex.Message;
                    return result;
                }
                finally
                {
                    sqlHelper.Dispose();
                }

            }

            //DBRKD DBCKD
            if (jobPara.jobCode == "10009")
            {
                try
                {


                    if (string.IsNullOrEmpty(serverUrl) || string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(appKey) || string.IsNullOrEmpty(appSecret) || string.IsNullOrEmpty(Pin))
                    {
                        result = "数据源中缺少serverUrl、accessToken、appKey、appSecret、Pin!";
                        return result;

                    }
                    else
                    {
                        client = new DefaultJdClient(serverUrl, accessToken, appKey, appSecret);
                    }
                    for (int i = 0; i < dsData.Count; i++)
                    {
                        var OrderLines = new List<Jdwl.Api.Domain.Clps.ClpsOpenGwService.PoItemModel>();
                        for (int j = 0; j < dsData[i].DataDetail[0].Rows.Count; j++)
                        {
                            OrderLines.Add(new Jdwl.Api.Domain.Clps.ClpsOpenGwService.PoItemModel
                            {
                                OrderLineNo = dsData[i].DataDetail[0].Rows[j]["DETAILNO"].SqlDataBankToString(),
                                ItemNo = dsData[i].DataDetail[0].Rows[j]["CLPSGOODSCODE"].SqlDataBankToString(),
                                PlanQty = dsData[i].DataDetail[0].Rows[j]["QUANTITY"].SqlDataBankToInt(),
                                GoodsStatus = "1"
                            });
                        }

                        var request = new Jdwl.Api.Request.Clps.ClpsAddPoOrderLopRequest
                        {
                            Pin = Pin,
                            PoAddRequest = new Jdwl.Api.Domain.Clps.ClpsOpenGwService.PoAddRequest
                            {
                                EntryOrder = new Jdwl.Api.Domain.Clps.ClpsOpenGwService.PoModel
                                {
                                    EntryOrderCode = dsData[i].DataMain.Rows[0]["LSH"].SqlDataBankToString(),
                                    OwnerCode = dsData[i].DataMain.Rows[0]["ownerCode"].SqlDataBankToString(),
                                    //the supplier code in JdWms
                                    SupplierCode = dsData[i].DataMain.Rows[0]["SupplierCode"].SqlDataBankToString(),
                                    WarehouseCode = dsData[i].DataMain.Rows[0]["warehouseNo"].SqlDataBankToString(),
                                    RelatedOrderList = new List<Jdwl.Api.Domain.Clps.ClpsOpenGwService.RelatedOrder> {
                                        new Jdwl.Api.Domain.Clps.ClpsOpenGwService.RelatedOrder
                                        {
                                            OrderCode =  dsData[i].DataMain.Rows[0]["ORDERTYPE"].SqlDataBankToString(),
                                            OrderType =  dsData[i].DataMain.Rows[0]["ORDERTYPE"].SqlDataBankToString()
                                        }
                                    }
                                },

                                OrderLines = OrderLines
                            }
                        };
                        var jsonrequest = JsonConvert.SerializeObject(request);
                        //{"response":{"content":{"code":"1","createTime":"2020-04-09 14:11:02","entryOrderCode":"CPL4418047912171","flag":"success","message":"成功"}, "code":0}}
                        var response = client.Execute(request);

                        //如果请求执行正确,从这里获取强类型返回值
                        OrderResponseBody returnValue = JsonConvert.DeserializeObject<OrderResponseBody>(response.Body);
                        if (returnValue.response.code == 0)
                        {
                            //save rtsorderstatus to STOCKRETURNAPPROVE_XH_JDWMS table
                            var insertresult = sqlHelper.ExecSql($"insert into CKDBDZK_XH_JDWMS(LSH,SYNCDATE,DBRKCKDCODE,TYPE) values('{dsData[i].DataMain.Rows[0]["LSH"].SqlDataBankToString()}',{createdate},'{returnValue.response.content.entryOrderCode}','{dsData[i].DataMain.Rows[0]["ORDERTYPE"].SqlDataBankToString()}')");
                            if (string.IsNullOrEmpty(insertresult))
                            {
                                allResult += $"\r\n仓库调拨单{dsData[i].DataMain.Rows[0]["LSH"].SqlDataBankToString()}成功";
                            }
                            else
                            {
                                sqlHelper.ExecSql($"insert into ERRORDATA_XH_JDWMS(GUID,ERRORNO,ERRORTYPE,MAINFIELD,ERRORDATE,ERRORMSG) values('{Guid.NewGuid():N}',{dsData[i].DataMain.Rows[0]["LSH"].SqlDataBankToString()},'ckdb','LSH',{createdate},'{returnValue.response.content.message}')");
                                allResult += $"\r\n仓库调拨单{dsData[i].DataMain.Rows[0]["LSH"].SqlDataBankToString()}失败,原因:{returnValue.response.content.message + insertresult}";
                            }
                        }
                        //响应的原始报文,如果请求失败,从这里获取错误消息代码
                        else
                        {
                            allResult += $"\r\n仓库调拨单，入参:{JsonConvert.SerializeObject(request)},\r\n返回:{JsonConvert.SerializeObject(response)}";
                        }

                    }
                    return allResult;
                }
                catch (Exception ex)
                {

                    result = "仓库调拨单" + ex.Message;
                    return result;
                }
                finally
                {
                    sqlHelper.Dispose();
                }

            }

            //error order close
            if (jobPara.jobCode == "10010")
            {
                try
                {


                    if (string.IsNullOrEmpty(serverUrl) || string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(appKey) || string.IsNullOrEmpty(appSecret) || string.IsNullOrEmpty(Pin))
                    {
                        result = "数据源中缺少serverUrl、accessToken、appKey、appSecret、Pin!";
                        return result;

                    }
                    else
                    {
                        client = new DefaultJdClient(serverUrl, accessToken, appKey, appSecret);
                    }
                    for (int i = 0; i < dsData.Count; i++)
                    {
                        var request = new Jdwl.Api.Request.Clps.ClpsClpsOrderCancelLopRequest
                        {
                            Pin = Pin,
                            OrderCancelRequest = new Jdwl.Api.Domain.Clps.ClpsOpenGwService.OrderCancelRequest
                            {
                                WarehouseCode = dsData[i].DataMain.Rows[0]["warehouseNo"].SqlDataBankToString(),
                                OwnerCode = dsData[i].DataMain.Rows[0]["ownerCode"].SqlDataBankToString(),
                                OrderId = dsData[i].DataMain.Rows[0]["ORDERID"].SqlDataBankToString(),
                                OrderType = dsData[i].DataMain.Rows[0]["ORDERTYPE"].SqlDataBankToString()
                            }
                        };
                        var jsonrequest = JsonConvert.SerializeObject(request);
                        //{"response":{"content":{"code":"1","createTime":"2020-04-09 14:11:02","entryOrderCode":"CPL4418047912171","flag":"success","message":"成功"}, "code":0}}
                        var response = client.Execute(request);

                        //如果请求执行正确,从这里获取强类型返回值
                        OrderResponseBody returnValue = JsonConvert.DeserializeObject<OrderResponseBody>(response.Body);
                        if (returnValue.response.code == 0)
                        {
                            //save rtsorderstatus to STOCKRETURNAPPROVE_XH_JDWMS table
                            var insertresult = sqlHelper.ExecSql($"insert into CloseOrder_XH_JDWMS(BILLNO,SYNCDATE,CLPSCODE,TYPE) values('{dsData[i].DataMain.Rows[0]["BILLNO"].SqlDataBankToString()}',{createdate},'{ dsData[i].DataMain.Rows[0]["ORDERID"].SqlDataBankToString()}','{dsData[i].DataMain.Rows[0]["ORDERTYPE"].SqlDataBankToString()}')");
                            if (string.IsNullOrEmpty(insertresult))
                            {
                                allResult += $"\r\n异常订单关闭{dsData[i].DataMain.Rows[0]["BILLNO"].SqlDataBankToString()}成功";
                            }
                            else
                            {
                                sqlHelper.ExecSql($"insert into ERRORDATA_XH_JDWMS(GUID,ERRORNO,ERRORTYPE,MAINFIELD,ERRORDATE,ERRORMSG) values('{Guid.NewGuid():N}',{dsData[i].DataMain.Rows[0]["BILLNO"].SqlDataBankToString()},'djqx','BILLNO',{createdate},'{returnValue.response.content.message}')");
                                allResult += $"\r\n异常订单关闭{dsData[i].DataMain.Rows[0]["BILLNO"].SqlDataBankToString()}失败,原因:{returnValue.response.content.message + insertresult}";
                            }
                        }
                        //响应的原始报文,如果请求失败,从这里获取错误消息代码
                        else
                        {
                            allResult += $"\r\n异常订单关闭，入参:{JsonConvert.SerializeObject(request)},\r\n返回:{JsonConvert.SerializeObject(response)}";
                        }

                    }
                    return allResult;
                }
                catch (Exception ex)
                {

                    result = "异常订单关闭" + ex.Message;
                    return result;
                }
                finally
                {
                    sqlHelper.Dispose();
                }

            }

            //wdgj goods 数据源取已经同步至wms中的hh
            if (jobPara.jobCode == "10011")
            {
                try
                {

                    OpenApi wdgjOpenApi = new OpenApi
                    {
                        Appkey = wdgj_appkey,
                        AppSecret = wdgj_appsecret,
                        AccessToken = wdgj_accesstoken,
                        Timestamp = ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000).ToString(),
                        Method = "wdgj.goods.create",
                        Format = "json",
                        Versions = "1.0",
                    };


                    ArrayList arrayList = new ArrayList();
                    List<GoodsDatainfo> datainfo = new List<GoodsDatainfo>();
                    for (int i = 0; i < dsData.Count; i++)
                    {
                        datainfo.Add(new GoodsDatainfo
                        {
                            goodsno = dsData[i].DataMain.Rows[0]["HH"].SqlDataBankToString(),
                            goodsname = dsData[i].DataMain.Rows[0]["PM"].SqlDataBankToString(),
                            unit = dsData[i].DataMain.Rows[0]["PDW"].SqlDataBankToString(),
                            bgift = dsData[i].DataMain.Rows[0]["ISGIFT"].SqlDataBankToInt(),
                            bmultispec = "0",
                            bnegativestock = "0",
                            barcode = dsData[i].DataMain.Rows[0]["TM"].SqlDataBankToString(),
                            speclist = new List<specinfo>
                            {
                                new specinfo
                                {
                                    bblockup = "0",
                                    bfixcost = "0"
                                }
                            }
                        });
                        arrayList.Add(dsData[i].DataMain.Rows[0]["HH"].SqlDataBankToString());
                    }

                    WdgjGoodsModel wdgjGoodsModel = new WdgjGoodsModel
                    {
                        datalist = datainfo
                    };

                    string createGoodsJson = JsonConvert.SerializeObject(wdgjGoodsModel, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    wdgjOpenApi.AppParam.content = createGoodsJson;
                    string postresult = wdgjOpenApi.HttpPostString();
                    WdgjGoodsResponse wdgjGoodsResponse = JsonConvert.DeserializeObject<WdgjGoodsResponse>(postresult);
                    allResult += $"\r\n推送商品信息到笛佛,接口返回信息:{postresult}";
                    if (wdgjGoodsResponse.datalist?.Count > 0)
                    {
                        string falsehhs = "";
                        for (int i = 0; i < wdgjGoodsResponse.datalist.Count; i++)
                        {
                            if (falsehhs == "")
                            {
                                falsehhs = $"'{wdgjGoodsResponse.datalist[i].goodsno}'";
                            }
                            else
                            {
                                falsehhs += $",'{wdgjGoodsResponse.datalist[i].goodsno}'";
                            }
                            arrayList.Remove(wdgjGoodsResponse.datalist[i].goodsno);
                        }
                        string falseupdate = $"update yw_kck_xh_jdwms set ISWDGJ = '2' where hh in ({falsehhs})";
                        var updateresult = sqlHelper.ExecSql(falseupdate);
                        allResult += $"\r\n推送商品信息到笛佛,更新sql语句:{falseupdate}\r\n,返回信息:{updateresult}";
                    }

                    if (wdgjGoodsResponse.returncode == "0")
                    {
                        string successhhs = "";
                        for (int i = 0; i < arrayList.Count; i++)
                        {
                            if (successhhs == "")
                            {
                                successhhs = $"'{arrayList[i]}'";
                            }
                            else
                            {
                                successhhs += $",'{arrayList[i]}'";
                            }
                        }
                        string successupdate = $"update yw_kck_xh_jdwms set ISWDGJ = '1' where hh in ({successhhs})";
                        var updateresult = sqlHelper.ExecSql(successupdate);
                        allResult += $"\r\n推送商品信息到笛佛,更新sql语句:{successupdate},返回信息:{updateresult}";

                    }
                    return allResult;
                }
                catch (Exception ex)
                {
                    result = $"推送商品信息到笛佛{ ex.Message}";
                    return result;
                }
                finally
                {
                    sqlHelper.Dispose();
                }

            }

            //wdgj supplier no test
            if (jobPara.jobCode == "10012")
            {
                try
                {
                    OpenApi wdgjOpenApi = new OpenApi
                    {
                        Appkey = wdgj_appkey,
                        AppSecret = wdgj_appsecret,
                        AccessToken = wdgj_accesstoken,
                        Timestamp = ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000).ToString(),
                        Method = "wdgj.prover.create",
                        Format = "json",
                        Versions = "1.0"

                    };
                    ArrayList arrayList = new ArrayList();
                    List<SupplierDatainfo> supplierDatainfos = new List<SupplierDatainfo>();
                    for (int i = 0; i < dsData.Count; i++)
                    {
                        supplierDatainfos.Add(new SupplierDatainfo
                        {
                            providername = dsData[i].DataMain.Rows[0]["MC"].SqlDataBankToString(),
                            providerno = dsData[i].DataMain.Rows[0]["TJBH"].SqlDataBankToString(),
                            provideralias = dsData[i].DataMain.Rows[0]["MC"].SqlDataBankToString()
                        });

                        arrayList.Add(dsData[i].DataMain.Rows[0]["TJBH"].SqlDataBankToString());
                    }

                    WdgjSupplierModel wdgjSupplier = new WdgjSupplierModel
                    {
                        datalist = supplierDatainfos
                    };
                    string createSuppliersJson = JsonConvert.SerializeObject(wdgjSupplier, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    wdgjOpenApi.AppParam.content = createSuppliersJson;

                    string postresult = wdgjOpenApi.HttpPostString();
                    WdgjSupplierResponse supplierResponse = JsonConvert.DeserializeObject<WdgjSupplierResponse>(postresult);
                    allResult += $"\r\n推送供应商信息到笛佛,接口返回信息:{postresult}";
                    if (supplierResponse.datalist?.Count > 0)
                    {
                        string falsehhs = "";
                        for (int i = 0; i < supplierResponse.datalist.Count; i++)
                        {
                            if (falsehhs == "")
                            {
                                falsehhs = $"'{supplierResponse.datalist[i].providerno}'";
                            }
                            else
                            {
                                falsehhs += $",'{supplierResponse.datalist[i].providerno}'";
                            }
                            // remove the error hh
                            arrayList.Remove(supplierResponse.datalist[i].providerno);
                        }

                        string falseupdate = $"update GL_SUPER_XH_JDWMS set ISWDGJ = '2' where TJBH in({falsehhs})";
                        var updateresult = sqlHelper.ExecSql(falseupdate);
                        allResult += $"\r\n推送供应商信息到笛佛,更新sql语句:{falseupdate}\r\n,返回信息:{updateresult}";
                    }

                    if (supplierResponse.returncode == "0")
                    {
                        string successhhs = "";
                        for (int i = 0; i < arrayList.Count; i++)
                        {
                            if (successhhs == "")
                            {
                                successhhs = $"'{arrayList[i]}'";
                            }
                            else
                            {
                                successhhs += $",'{arrayList[i]}'";
                            }
                        }
                        string successupdate = $"update GL_SUPER_XH_JDWMS set ISWDGJ = '1' where TJBH in({successhhs})";
                        var updateresult = sqlHelper.ExecSql(successupdate);
                        allResult += $"\r\n推送供应商信息到笛佛,更新sql语句:{successupdate},返回信息:{updateresult}";

                    }
                    return allResult;
                }
                catch (Exception ex)
                {

                    result = $"推送供应商信息到笛佛{ex.Message}";
                    return result;
                }
                finally
                {
                    sqlHelper.Dispose();
                }
            }

            //消除错误，战胜恐惧，处理推送错误的数据
            if (jobPara.jobCode == "resetError")
            {
                result = "0";
                sqlHelper.ExecSql("delete ERRORDATA_XH_JDWMS");
                return "ERRORDATA_XH_JDWMS表已清空，推送错误的数据已重置";
            }

            else
            {
                return "";
            }
        }
    }
}
