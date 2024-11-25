namespace PlanGuruAPI.DTOs.PlantPostDTOs
{
    public record CreatePlantPostRequest(
        string Title,
        string Description,
        Guid UserId,
        string ImageUrl,
        string Tag,
        string Background
    );

}
