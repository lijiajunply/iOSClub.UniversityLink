using UniversityLink.DataModels;

namespace iOSClub.UniversityLink.Models;

public class AllDataModel
{
    public List<UserModel> Users { get; set; } = [];
    public List<CategoryModel> Categories { get; set; } = [];
}