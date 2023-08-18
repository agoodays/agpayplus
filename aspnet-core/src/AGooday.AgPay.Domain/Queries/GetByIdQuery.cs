using MediatR;

namespace AGooday.AgPay.Domain.Queries
{
    public class GetByIdQuery<TEntity, TPrimaryKey> : IRequest<TEntity>
    {
        public TPrimaryKey Id { get; private set; }

        public GetByIdQuery(TPrimaryKey id)
        {
            this.Id = id;
        }
    }
}
