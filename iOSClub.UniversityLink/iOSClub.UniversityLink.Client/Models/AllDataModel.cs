using UniversityLink.DataModels;

namespace iOSClub.UniversityLink.Client.Models;

public class AllDataModel
{
    public List<UserModel> Users { get; init; } = [];
    public List<CategoryModel> Categories { get; init; } = [];
}