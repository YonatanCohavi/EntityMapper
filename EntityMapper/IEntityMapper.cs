using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace EntityMapper
{
    public interface IEntityMapper
    {
        ColumnSet GetColumnSet<T>() where T : CrmEntityBase, new();
        Entity ToEntity(CrmEntityBase model);
        T ToModel<T>(Entity entity, string entityAlias = null) where T : CrmEntityBase, new();
    }
}