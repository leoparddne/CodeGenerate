using System.Collections.Generic;

namespace Service.IService.Scheme.Oracle
{
    public interface IUserConstraintsService
    {
        /// <summary>
        /// ��ȡָ���������ֶ�
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        List<string> GetPK(string table, string configID);
    }
}


