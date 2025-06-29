using System.Text;
using Backoffice.Domain.OperationRequests;
using Backoffice.Domain.Shared;
using Backoffice.Domain.Staffs;
using Backoffice.Domain.SurgeryRooms;
using Backoffice.Domain.SurgeryRooms.ValueObjects;

namespace Backoffice.Domain.SurgeryRooms
{
    public class SurgeryRoomService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISurgeryRoomRepository _repo;
        private readonly IOperationRequestRepository _opreqrepo;
        private readonly IStaffRepository _staffrepo;
        private readonly ISurgeryRoomRepository _roomrepo;
        private readonly ISurgeryRoomTypeRepository _roomTypeRepo;

        //private readonly ILogRepository _logrepo;

        public SurgeryRoomService(IUnitOfWork unitOfWork, ISurgeryRoomRepository repo,
                                        IOperationRequestRepository opreqrepo,
                                        IStaffRepository doctorrepo, ISurgeryRoomRepository roomrepo,
                                        ISurgeryRoomTypeRepository _roomTypeRepository
                                        /*ILogRepository logrepo*/)
        {
            _unitOfWork = unitOfWork;
            _repo = repo;
            _opreqrepo = opreqrepo;
            _roomrepo = roomrepo;
            _staffrepo = doctorrepo;
            _roomTypeRepo = _roomTypeRepository;
            //_logrepo = logrepo;
        }
        
        public async Task<List<SurgeryRoomDto>> GetAllAsync()
        {
            var list = await _repo.GetAllSurgeryRoomsAsync();

            List<SurgeryRoomDto> listDto = new List<SurgeryRoomDto>();
            //StringBuilder rooms = new StringBuilder();

            foreach (var item in list)
            {
                listDto.Add(SurgeryRoomMapper.ToDto(item, item.SurgeryRoomType));
                //rooms.Append(item.SurgeryRoomType.Id.ToString());
                //rooms.Append("/");
            }

            //throw new Exception(rooms.ToString());

            return listDto;
            //return null;
        }

        public async Task<SurgeryRoomDto> GetByNumberAsync(int number)
        {
            var SurgeryRoom = await _repo.GetSurgeryRoomByRoomNumberAsync(number);

            if (SurgeryRoom == null) throw new BusinessRuleValidationException("Error: The surgery room doesn't exist.");

            return SurgeryRoomMapper.ToDto(SurgeryRoom, SurgeryRoom.SurgeryRoomType);
        }
        // public async Task<SurgeryRoomDto> CreateAsync(CreateSurgeryRoomDto dto)
        // {
        //     var operationRequest = await _opreqrepo.GetOpRequestByIdAsync(new OperationRequestId(dto.OpRequestId));
        //     if (operationRequest == null) throw new BusinessRuleValidationException("Error: The operation request doesn't exist.");

        //     if (string.IsNullOrEmpty(dto.StaffIds))
        //     {
        //         throw new BusinessRuleValidationException("Error: The staff must be selected.");
        //     }

        //     // Split the input string by the semicolon separator
        //     string[] ids = dto.StaffIds.Split(';');

        //     // Create a list to hold the parsed GUIDs
        //     List<StaffId> staffIdList = new List<StaffId>();

        //     // Iterate through each ID string, parse it to a GUID, and add to the list
        //     foreach (string id in ids)
        //     {
        //         // Trim any leading or trailing whitespace from each ID string
        //         string trimmedId = id.Trim();

        //         // Parse the trimmed ID string to a GUID and add to the list
        //         if (Guid.TryParse(trimmedId, out Guid parsedGuid))
        //         {
        //             staffIdList.Add(new StaffId(parsedGuid));
        //         }
        //         else
        //         {
        //             // Optionally, handle the case where parsing fails (e.g., log an error)
        //             Console.WriteLine($"Invalid GUID format: {trimmedId}");
        //         }
        //     }

        //     var staffList = await _staffrepo.GetByIdsAsync(staffIdList);

        //     var surgeryRoom = await _roomrepo.GetSurgeryRoomByIdAsync(new SurgeryRoomId(dto.SurgeryRoomId));
        //     if (surgeryRoom == null) throw new BusinessRuleValidationException("Error: The surgery room doesn't exist.");

        //     var SurgeryRoom = SurgeryRoomMapper.ToDomain(dto, operationRequest, staffList, surgeryRoom);

        //     await _repo.AddAsync(SurgeryRoom);
        //     await _unitOfWork.CommitAsync();

        //     return SurgeryRoomMapper.ToDto(SurgeryRoom);
        // }


        public async Task<bool> addRoomType(string type, bool? fitForSurgery, string description){

            if (((await this._roomTypeRepo.SurgeryRoomTypeExists(type))))
            {
                throw new BusinessRuleValidationException("Error: This type already exists.");
            }

            await _roomTypeRepo.AddAsync(new SurgeryRoomType(type, fitForSurgery ,description));

            await this._unitOfWork.CommitAsync();

            return true;
        }

        public async Task<List<string>> GetAllRoomTypeAsync()
        {
            return await this._roomTypeRepo.getTypeList();
        }

        public async Task<List<SurgeryRoomTypeDto>> GetAllRoomTypeDtoAsync()
        {
            return await this._roomTypeRepo.getTypeListDTO();
        }

        
    }
}