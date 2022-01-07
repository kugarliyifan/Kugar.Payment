# 本类库主要用于微支付的接口封装
# 默认的回调通知地址为  支付回调= /Core/Payment/Callback/Wechatpay/{appID}  退款通知回调地址为:/Core/Refund/Callback/Wechatpay/{appID}
# 实际发送数据时,会自动替换 {appID} ,如需要使用默认的回调处理,需引用 Kugar.Payment.Wechatpay.Web 类库,并且 注入 IResultNotifyHandler 接口,在微支付回调后,会自动触发IResultNotifyHandler接口对应的函数

```c#
public void ConfigureServices(IServiceCollection services)
{
	//方法一 :  全局静态Factory,适合多AppId的情况下使用
	WechatpayFactory.AddConfig(new WechatpayConfig(){
		//初始化支付参数
		Host="" //当前域名配置,如果不配置则取当前请求的域名,可以传入String指明固定的域名,也可传入回调函数,回调时,根据当前Request,判断返回的域名,需要注意的是,返回的域名必须带http/https
	})

	//方法二: 适合只有单个AppId
	services.AddSingleton<Wechatpay>(x=>new Wechatpay(new WechatpayConfig() {} ))


	services.AddScoped<IResultNotifyHandler,T>();  //注入回调处理函数
}


```




在调用的时候:
```c#
public class A
{

	public async Task<IActionnResult> Test1()
	{
		var service=WechatpayFactory.GetByAppId(""); //传入AppId

		var result=await service.Micropay().填入所需参数.ExecuteAsync()  //根据支付类型,调用不同支付服务类
	}
}
```