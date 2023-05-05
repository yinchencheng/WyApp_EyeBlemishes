using System;
using System.Threading;
using HslCommunication;
using HslCommunication.Core.Net;
using HslCommunication.Profinet.Melsec;
using HslCommunication.Profinet.Omron;
using HslCommunication.Profinet.Siemens;
using HslCommunication.Profinet.Inovance;
using WY_App.Utility;
using HslCommunication.Core;
using System.IO;

using HslCommunication.ModBus;
//power pmac配置
using ODT.PowerPmacComLib;
using ODT.Common.Services;
using ODT.Common.Core;
using System.Windows.Forms;
using System.Collections.Generic;
using static WY_App.Utility.Parameter;
using static OpenCvSharp.FileStorage;
using static System.Collections.Specialized.BitVector32;
using Newtonsoft.Json.Linq;

namespace WY_App
{
    internal class HslCommunication
    {
        public static OperateResult _connected ;
        static NetworkDeviceBase _NetworkTcpDevice;
        static ISyncGpasciiCommunicationInterface communication = null;
        deviceProperties currentDeviceProp = new deviceProperties();
        deviceProperties currentDevProp = new deviceProperties();
        string commands = String.Empty;
        string response = String.Empty;
        public static bool plc_connect_result = false;
        public static ModbusRtu busRtuClient;

        public HslCommunication()
        {
            try
            {
                Parameter.plcParams = XMLHelper.BackSerialize<Parameter.PLCParams>("Parameter/PLCParams.xml");
            }
            catch
            {
                Parameter.plcParams = new Parameter.PLCParams();
                XMLHelper.serialize<Parameter.PLCParams>(Parameter.plcParams, "Parameter/PLCParams.xml");
            }
           
            Thread th = new Thread(ini_PLC_Connect);
            th.IsBackground = true;
            th.Start();

            Thread PLC_Read = new Thread(ini_PLC_Read);
            PLC_Read.IsBackground = true;
            PLC_Read.Start();
        }
        public void ini_PLC_Connect()
        {
            if (!Authorization.SetAuthorizationCode("f562cc4c-4772-4b32-bdcd-f3e122c534e3"))
            {
                LogHelper.Log.WriteError("HslCommunication 组件认证失败，组件只能使用8小时!");
                MainForm.AlarmList.Add("HslCommunication 组件认证失败，组件只能使用8小时!");
            }           
            while (!plc_connect_result)
            {
                //Thread.Sleep(5000);
                try
                {
                    //欧姆龙PLC Omron.PMAC.CK3M通讯
                    if ("Omron.PMAC.CK3M".Equals(Parameter.commministion.PlcType))
                    {
                        currentDevProp.IPAddress = Parameter.commministion.PlcIpAddress;
                        currentDevProp.Password = "deltatau";
                        currentDevProp.PortNumber = Parameter.commministion.PlcIpPort;
                        currentDevProp.User = "root";
                        currentDevProp.Protocol = CommunicationGlobals.ConnectionTypes.SSH;
                        communication = Connect.CreateSyncGpascii(currentDevProp.Protocol, communication);
                        plc_connect_result = communication.ConnectGpAscii(currentDevProp.IPAddress, currentDevProp.PortNumber, currentDevProp.User, currentDevProp.Password);                                              
                    }
                    //欧姆龙PLC OmronFinsNet通讯
                    else if ("Omron.OmronFinsNet".Equals(Parameter.commministion.PlcType))
                    {
                        OmronFinsNet Client = new OmronFinsNet();
                        Client.IpAddress = Parameter.commministion.PlcIpAddress;
                        Client.Port = Parameter.commministion.PlcIpPort;
                        Client.SA1 = Convert.ToByte(Parameter.commministion.PlcDevice);
                        Client.ConnectTimeOut = 5000;
                        _NetworkTcpDevice = Client;
                        _connected = _NetworkTcpDevice.ConnectServer();
                        plc_connect_result = _connected.IsSuccess;
                    }
                    //三菱PLC Melsec.MelsecMcNet通讯
                    else if ("Melsec.MelsecMcNet".Equals(Parameter.commministion.PlcType))
                    {
                        MelsecMcNet Client = new MelsecMcNet();
                        Client.IpAddress = Parameter.commministion.PlcIpAddress;
                        Client.Port = Parameter.commministion.PlcIpPort;
                        Client.ConnectTimeOut = 5000;
                        _NetworkTcpDevice = Client;
                        _connected = _NetworkTcpDevice.ConnectServer();
                        plc_connect_result = _connected.IsSuccess;
                    }
                  
                    //西门子PLC Siemens.SiemensS7Net通讯
                    else if ("Siemens.SiemensS7Net".Equals(Parameter.commministion.PlcType))
                    {
                        SiemensS7Net Client = new SiemensS7Net((SiemensPLCS)Convert.ToInt16(Parameter.commministion.PlcDevice), Parameter.commministion.PlcIpAddress);
                        Client.IpAddress = Parameter.commministion.PlcIpAddress;
                        Client.Port = Parameter.commministion.PlcIpPort;
                        Client.ConnectTimeOut = 5000;
                        _NetworkTcpDevice = Client;
                        _connected = _NetworkTcpDevice.ConnectServer();
                        plc_connect_result = _connected.IsSuccess;
                    }
                    //汇川PLC Inovance.InovanceSerialOverTcp通讯
                    else if ("Inovance.InovanceSerialOverTcp".Equals(Parameter.commministion.PlcType))
                    {
                        InovanceSerialOverTcp Client = new InovanceSerialOverTcp();
                        Client.IpAddress = Parameter.commministion.PlcIpAddress;
                        Client.Port = Parameter.commministion.PlcIpPort;
                        Client.DataFormat = DataFormat.ABCD;
                        Client.ConnectTimeOut = 5000;
                        _NetworkTcpDevice = Client;
                        _connected = _NetworkTcpDevice.ConnectServer();
                        plc_connect_result = _connected.IsSuccess;
                    }
                    //ModbusTcp通讯
                    else if ("ModbusTcpNet".Equals(Parameter.commministion.PlcType))
                    {
                        ModbusTcpNet Client = new ModbusTcpNet();
                        Client.IpAddress = Parameter.commministion.PlcIpAddress;
                        Client.Port = Parameter.commministion.PlcIpPort;
                        Client.Station = 0x01;
                        Client.ConnectTimeOut = 5000;
                        _NetworkTcpDevice = Client;
                        _connected = _NetworkTcpDevice.ConnectServer();
                        plc_connect_result = _connected.IsSuccess;
                    }
                    //新增通讯添加else if判断创建连接
                    else if ("ModbusRtu".Equals(Parameter.commministion.PlcType))
                    {
                        try
                        {
                            busRtuClient = new ModbusRtu(Convert.ToByte(Parameter.commministion.PlcDevice));
                            busRtuClient.SerialPortInni(sp =>
                            {
                                sp.PortName = Parameter.commministion.PlcIpAddress;
                                sp.BaudRate = Parameter.commministion.PlcIpPort;
                                sp.DataBits = 8;
                                sp.StopBits = System.IO.Ports.StopBits.One;
                                sp.Parity = System.IO.Ports.Parity.None;                              
                            });
                            busRtuClient.Open(); // 打开
                            plc_connect_result = true;
                        }
                        catch (Exception ex)
                        {
                            plc_connect_result = false;
                        }
                        if (plc_connect_result)
                        {
                            LogHelper.Log.WriteInfo(Parameter.commministion.PlcType + "串口" + Parameter.commministion.PlcIpAddress + "打开成功,波特率:" + Parameter.commministion.PlcIpPort);
                            MainForm.AlarmList.Add(Parameter.commministion.PlcType + "串口" + Parameter.commministion.PlcIpAddress + "打开成功,波特率:" + Parameter.commministion.PlcIpPort);
                            plc_connect_result = true;
                        }
                        else
                        {
                            LogHelper.Log.WriteError(Parameter.commministion.PlcType + "串口" + Parameter.commministion.PlcIpAddress + "打开失败,波特率:" + Parameter.commministion.PlcIpPort);
                            MainForm.AlarmList.Add(Parameter.commministion.PlcType + "串口" + Parameter.commministion.PlcIpAddress + "打开失败,波特率:" + Parameter.commministion.PlcIpPort);
                            plc_connect_result = false;
                        }
                        
                    }
                    //Parameter.PlcType字符错误或未定义
                    else
                    {
                        LogHelper.Log.WriteError(Parameter.commministion.PlcType + "类型未定义!!!");
                        MainForm.AlarmList.Add(Parameter.commministion.PlcType + "类型未定义!!!");
                        plc_connect_result = false;
                       
                    }
                   
                    if (plc_connect_result)
                    {
                        LogHelper.Log.WriteInfo(Parameter.commministion.PlcType + "连接成功:IP" + Parameter.commministion.PlcIpAddress + "  Port:" + Parameter.commministion.PlcIpPort);
                        MainForm.AlarmList.Add(Parameter.commministion.PlcType + "连接成功:IP" + Parameter.commministion.PlcIpAddress + "  Port:" + Parameter.commministion.PlcIpPort);
                        plc_connect_result = true;
                    }
                    else
                    {
                        LogHelper.Log.WriteError(Parameter.commministion.PlcType + "连接失败:IP" + Parameter.commministion.PlcIpAddress + "  Port:" + Parameter.commministion.PlcIpPort);
                        MainForm.AlarmList.Add(Parameter.commministion.PlcType + "连接失败:IP" + Parameter.commministion.PlcIpAddress + "  Port:" + Parameter.commministion.PlcIpPort);
                        plc_connect_result = false;
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Log.WriteError(Parameter.commministion.PlcType + "初始化失败:", ex.Message);
                    MainForm.AlarmList.Add(Parameter.commministion.PlcType + "初始化失败:"+ ex.Message);
                    plc_connect_result = false;
                }
            }
            while (plc_connect_result)
            {
                ////心跳读写，判断PLC是否掉线，不建议线程对plc链接释放重连
                if ("Omron.PMAC.CK3M".Equals(Parameter.commministion.PlcType))
                {
                    plc_connect_result = ReadWritePmacVariables(Parameter.plcParams.HeartBeatAdd);
                    Thread.Sleep(5000);
                }
                else if ("ModbusRtu".Equals(Parameter.commministion.PlcType))
                {
                    try
                    {
                        if (Parameter.plcParams.HeartBeatAdd != null || Parameter.plcParams.HeartBeatAdd != "")
                        {
                            _connected = busRtuClient.Write(Parameter.plcParams.HeartBeatAdd, 1);
                            Thread.Sleep(1000);
                            _connected = busRtuClient.Write(Parameter.plcParams.HeartBeatAdd, 0);
                            Thread.Sleep(1000);
                            plc_connect_result = true;
                        }
                        else
                        {
                            Thread.Sleep(100000);
                        }

                    }
                    catch
                    {
                        busRtuClient.Dispose();
                        plc_connect_result = false;
                    }
                }
                else
                {
                    try
                    {
                        if(Parameter.plcParams.HeartBeatAdd!=null|| Parameter.plcParams.HeartBeatAdd!="")
                        {
                            _connected = _NetworkTcpDevice.Write(Parameter.plcParams.HeartBeatAdd, 1);
                            Thread.Sleep(1000);
                            _connected = _NetworkTcpDevice.Write(Parameter.plcParams.HeartBeatAdd, 0);
                            Thread.Sleep(1000);
                            plc_connect_result = true;
                        }
                        else
                        {
                            Thread.Sleep(100000);
                        }
                        
                    }
                    catch
                    {
                        _NetworkTcpDevice.Dispose();
                        _connected.IsSuccess = false;
                        plc_connect_result = false;                        
                    }
                }
            }             
        }
        public void ini_PLC_Read()
        { 
            while(true)
            {
                if(plc_connect_result)
                {
                    if ("Omron.PMAC.CK3M".Equals(Parameter.commministion.PlcType))
                    {

                    }
                    else if ("ModbusRut".Equals(Parameter.commministion.PlcType))
                    {

                    }
                    else
                    {
                        
                    }
                }
                else
                {
                    Thread.Sleep(1000);
                  
                }                
            }
        }


        //对终端操作的通用方法/
        public bool ReadWritePmacVariables(string command)
        {
            var commads = new List<string>();
            List<string> responses;
            commads.Add(command.ToString());
            var communicationStatus = communication.GetResponse(commads, out responses, 3);
            if (communicationStatus == Status.Ok)
            {
                response = string.Join("", responses.ToArray());
                command = null;
                return  true;
            }
            else
            {
                return  false;
            }
        }


        public void Read(string address,int value)
        {
            if (plc_connect_result)
            {
                if ("Omron.PMAC.CK3M".Equals(Parameter.commministion.PlcType))
                {

                }
                else if ("ModbusRut".Equals(Parameter.commministion.PlcType))
                {
                    busRtuClient.Write(address, value);
                }
                else
                {
                    _NetworkTcpDevice.Write(address, value);
                }
            }
        }
        public static string  Read(string address)
        {
            if (plc_connect_result)
            {
                if ("Omron.PMAC.CK3M".Equals(Parameter.commministion.PlcType))
                {
                    return null;
                }
                else if ("ModbusRut".Equals(Parameter.commministion.PlcType))
                {

                    string value = busRtuClient.ReadUInt16(address).ToString();
                    return value;
                }
                else
                {
                    string value = _NetworkTcpDevice.ReadUInt16(address).ToString();
                    return value;
                }
            }
            else
                return null;
        }
        public static UInt16 ReadUInt16(string address)
        {
            if (plc_connect_result)
            {
                if ("Omron.PMAC.CK3M".Equals(Parameter.commministion.PlcType))
                {
                    return 0;
                }
                else if ("ModbusRtu".Equals(Parameter.commministion.PlcType))
                {

                    UInt16 value = busRtuClient.ReadUInt16(address).Content;
                    return value;
                }
                else
                {
                    UInt16 value = _NetworkTcpDevice.ReadUInt16(address).Content;
                    return value;
                }
            }
            else
                return 0;
        }
        public static void Write(string address, bool value)
        {
            if (plc_connect_result)
            {
                if ("Omron.PMAC.CK3M".Equals(Parameter.commministion.PlcType))
                {

                }
                else if ("ModbusRtu".Equals(Parameter.commministion.PlcType))
                {

                    _connected = busRtuClient.Write(address, value);
                    LogHelper.Log.WriteError(Parameter.commministion.PlcType + "数据写入:", _connected.Message + ",地址:"+address + "值:"+value);
                    MainForm.AlarmList.Add(Parameter.commministion.PlcType + "数据写入:"+ _connected.Message + ",地址:" + address + "值:" + value);

                }
                else
                {
                    _NetworkTcpDevice.Write(address, value);
                    LogHelper.Log.WriteError(Parameter.commministion.PlcType + "数据写入:", _connected.Message + ",地址:" + address + "值:" + value);
                    MainForm.AlarmList.Add(Parameter.commministion.PlcType + "数据写入:" + _connected.Message + ",地址:" + address + "值:" + value);
                }
            }
        }
        public static void Write(string address, int value)
        {
            if (plc_connect_result)
            {
                if ("Omron.PMAC.CK3M".Equals(Parameter.commministion.PlcType))
                {

                }
                else if ("ModbusRtu".Equals(Parameter.commministion.PlcType))
                {

                    busRtuClient.Write(address, value);
                    LogHelper.Log.WriteError(Parameter.commministion.PlcType + "数据写入:", _connected.Message + ",地址:" + address + "值:" + value);
                    MainForm.AlarmList.Add(Parameter.commministion.PlcType + "数据写入:" + _connected.Message + ",地址:" + address + "值:" + value);
                }
                else
                {
                    _NetworkTcpDevice.Write(address, value);
                    LogHelper.Log.WriteError(Parameter.commministion.PlcType + "数据写入:", _connected.Message + ",地址:" + address + "值:" + value);
                    MainForm.AlarmList.Add(Parameter.commministion.PlcType + "数据写入:" + _connected.Message + ",地址:" + address + "值:" + value);
                }
            }
        }
        public static void Write(string address, string value)
        {
            if (plc_connect_result)
            {
                if ("Omron.PMAC.CK3M".Equals(Parameter.commministion.PlcType))
                {
                    busRtuClient.Write(address, value);
                    LogHelper.Log.WriteError(Parameter.commministion.PlcType + "数据写入:", _connected.Message + ",地址:" + address + "值:" + value);
                    MainForm.AlarmList.Add(Parameter.commministion.PlcType + "数据写入:" + _connected.Message + ",地址:" + address + "值:" + value);
                }
                else
                {
                    _NetworkTcpDevice.Write(address, value);
                    LogHelper.Log.WriteError(Parameter.commministion.PlcType + "数据写入:", _connected.Message + ",地址:" + address + "值:" + value);
                    MainForm.AlarmList.Add(Parameter.commministion.PlcType + "数据写入:" + _connected.Message + ",地址:" + address + "值:" + value);
                }
            }
        }
    }
}
