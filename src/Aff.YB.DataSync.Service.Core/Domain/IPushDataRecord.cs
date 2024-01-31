namespace Aff.YB.DataSync.Service.Core.Domain
{
    public interface IPushDataRecord
    {
        string DataSource { get; }

        string XmlData { get; }

        int ErrCode { get; }

        string ErrMsg { get; }
    }
}
