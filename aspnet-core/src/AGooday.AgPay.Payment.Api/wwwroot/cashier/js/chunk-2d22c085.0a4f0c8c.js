(window["webpackJsonp"]=window["webpackJsonp"]||[]).push([["chunk-2d22c085"],{f203:function(e,t,o){"use strict";o.r(t);var n=function(){var e=this;e._self._c;return e._m(0)},s=[function(){var e=this,t=e._self._c;return t("div",[t("p",{staticStyle:{"font-size":"16px"}},[e._v("正在跳转...")])])}],r=(o("14d9"),o("4ec3")),a=o("f7a4"),c=o("53f4"),i=o("f121"),u={components:{},mounted(){console.log("正在跳转",this.$route.params,this.$route.query);const e=Object.assign({},this.searchToObject(),this.$route.query);console.log(e);const t=this;Object(r["a"])(e).then(e=>{c["a"].setChannelUserId(e),this.$router.push({name:a["a"].getPayWay().routeName})}).catch(e=>{t.$router.push({name:i["a"].errorPageRouteName,params:{errInfo:e.msg}})})},methods:{searchToObject:function(){if(!window.location.search)return{};let e,t,o=window.location.search.substring(1).split("&"),n={};for(t in o)""!==o[t]&&(e=o[t].split("="),n[decodeURIComponent(e[0])]=decodeURIComponent(e[1]));return n}}},h=u,l=o("2877"),p=Object(l["a"])(h,n,s,!1,null,null,null);t["default"]=p.exports}}]);
//# sourceMappingURL=chunk-2d22c085.0a4f0c8c.js.map