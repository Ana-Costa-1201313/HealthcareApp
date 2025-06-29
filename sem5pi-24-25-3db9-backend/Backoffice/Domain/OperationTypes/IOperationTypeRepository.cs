using Backoffice.Domain.Shared;

namespace Backoffice.Domain.OperationTypes
{
    public interface IOperationTypeRepository : IRepository<OperationType, OperationTypeId>
    {
        public Task<List<OperationType>> GetAllWithDetailsAsync();
        public Task<OperationType> GetByIdWithDetailsAsync(OperationTypeId id);
        public Task<OperationType> GetByIdWithDetailsSuperAsync(OperationTypeId id);
        public Task<bool> OperationTypeNameExists(string name);
        public Task<List<OperationType>> FilterOperationTypesAsync(string name, string specialization, bool? status);
        public Task<OperationType> GetByOperationTypeName(string name);
    }
}