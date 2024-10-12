using Base.Models;
using Base.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EarthApi.Services
{
    //for dropdown input
    public static class _XpCode
    {
        #region master table to codes
        public static async Task<List<IdStrDto>?> ItemTypesA(Db? db = null)
        {
            return await _Db.TableToCodes2A("ItemType", "Sort", db);
        }
        public static async Task<List<IdStrExtDto>?> ItemsA(Db? db = null)
        {
            return await _Db.TableToCodeExts2A("Item", "TypeId", "Sort", db);
        }
        public static async Task<List<IdStrDto>?> RolesA(Db? db = null)
        {
            return await _Db.TableToCodes2A("XpRole", "Sort", db);
        }
        public static async Task<List<IdStrDto>?> ProgsA(Db? db = null)
        {
            return await _Db.TableToCodes2A("XpProg", "Sort", db);
        }
        public static async Task<List<IdStrDto>?> SkillsA(Db? db = null)
        {
            return await _Db.TableToCodesA("Skill", db);
        }
        #endregion

        /*
        #region get from XpCode table
        public static async Task<List<IdStrDto>> AuthRangesA(string locale0, Db? db)
        {
            return await _Db.TypeToCodesA("AuthRange", db, locale0);
        }
        #endregion
        */

    }//class
}
