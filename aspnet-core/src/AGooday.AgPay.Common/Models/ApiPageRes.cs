namespace AGooday.AgPay.Common.Models
{
    /// <summary>
    /// 接口返回分页对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiPageRes<T> : ApiRes
    {
        /// <summary>
        /// 业务数据
        /// </summary>
        public new PageBean<T> Data { get; set; }

        /// <summary>
        /// 业务处理成功， 封装分页数据， 仅返回必要参数
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ApiPageRes<T> Pages(PaginatedResult<T> data)
        {
            PageBean<T> innerPage = new PageBean<T>();
            innerPage.Records = data.Items; //记录明细
            innerPage.Total = data.TotalCount; //总条数
            innerPage.Current = data.PageIndex; //当前页码
            innerPage.HasNext = data.HasNextPage; //是否有下一页

            ApiPageRes<T> result = new ApiPageRes<T>();
            result.Data = innerPage;
            result.Code = ApiCode.SUCCESS.GetCode();
            result.Msg = ApiCode.SUCCESS.GetMsg();

            return result;
        }
    }

    public class PageBean<T>
    {
        /// <summary>
        /// 数据列表
        /// </summary>
        public IList<T> Records { get; set; }

        /// <summary>
        /// 总数量
        /// </summary>
        public long Total { get; set; }

        /// <summary>
        /// 当前页码
        /// </summary>
        public long Current { get; set; }

        /// <summary>
        /// 是否包含下一页， true:包含 ，false: 不包含
        /// </summary>
        public bool HasNext { get; set; }
    }
}
