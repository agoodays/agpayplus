export const printANSI = () => {
  let text = `
   __    ___  ____   __   _  _ 
  /__\\  / __)(  _ \\ /__\\ ( \\/ )
 /(__)\\( (_-. )___//(__)\\ \\  / 
(__)(__)\\___/(__) (__)(__)(__) 

 :: AgPay ::        (v1.0.0.RELEASE)
 适合互联网企业使用的开源支付系统 : https://www.agooday.com
`
  console.log(`%c${text}`, 'color: #fc4d50')
  console.log('%cThanks for using AgPay!', 'color: #fff; font-size: 14px; font-weight: 300; text-shadow:#000 1px 0 0,#000 0 1px 0,#000 -1px 0 0,#000 0 -1px 0;')
}
