using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace OnlineStore.Models.ViewModel;

public class ImportExportModel
{
    public IFormFileCollection Files { get; set; }
    public SelectList Formats { get; set; }
}