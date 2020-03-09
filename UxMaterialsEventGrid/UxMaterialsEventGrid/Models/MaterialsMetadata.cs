using Microsoft.WindowsAzure.Storage.Table;

namespace UxMaterialsEventGrid.Models
{
    public class MaterialsMetadata : TableEntity
    {
        public string Path { get; set; }
        public string ContentType { get; set; }
        public int ContentLength { get; set; }
    }
}
