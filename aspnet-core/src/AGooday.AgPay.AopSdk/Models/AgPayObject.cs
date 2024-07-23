namespace AGooday.AgPay.AopSdk.Models
{
    public class AgPayObject
    {
        /// <summary>
        /// 其他拓展信息
        /// </summary>
        protected Dictionary<string, object> extendInfos = new Dictionary<string, object>();

        /// <summary>
        /// 获取拓展参数
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> GetExtendInfos()
        {
            return extendInfos;
        }

        /// <summary>
        /// 新增拓展参数
        /// 扩展参数级别高于实体参数，会覆盖已有的Key，注意大小写
        /// </summary>
        /// <param name="extendInfos"></param>
        public void SetExtendInfo(Dictionary<string, object> extendInfos)
        {
            foreach (var ext in extendInfos)
            {
                this.extendInfos.Add(ext.Key, ext.Value);
            }
        }

        /// <summary>
        /// 新增拓展参数
        /// 扩展参数级别高于实体参数，会覆盖已有的Key，注意大小写
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddExtendInfo(string key, object value)
        {
            this.extendInfos.Add(key, value);
        }
    }
}
