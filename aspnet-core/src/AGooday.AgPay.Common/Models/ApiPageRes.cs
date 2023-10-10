namespace AGooday.AgPay.Common.Models
{
    /// <summary>
    /// 接口返回分页对象
    /// </summary>
    /// <typeparam name="M"></typeparam>
    public class ApiPageRes<M> : ApiRes
    {
        /// <summary>
        /// 业务数据
        /// </summary>
        public new PageBean<M> Data { get; set; }

        /// <summary>
        /// 业务处理成功， 封装分页数据， 仅返回必要参数
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ApiPageRes<M> Pages(PaginatedList<M> data)
        {
            PageBean<M> innerPage = new PageBean<M>();
            innerPage.Records = data.ToList(); //记录明细
            innerPage.Total = data.TotalCount; //总条数
            innerPage.Current = data.PageIndex; //当前页码
            innerPage.HasNext = data.HasNext; //是否有下一页

            ApiPageRes<M> result = new ApiPageRes<M>();
            result.Data = innerPage;
            result.Code = ApiCode.SUCCESS.GetCode();
            result.Msg = ApiCode.SUCCESS.GetMsg();

            return result;
        }
    }

    public class PageBean<M>
    {
        /// <summary>
        /// 数据列表
        /// </summary>
        public List<M> Records { get; set; }

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
