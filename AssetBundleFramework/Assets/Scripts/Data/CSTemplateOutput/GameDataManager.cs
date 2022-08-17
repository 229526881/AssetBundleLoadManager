/**
 * Auto generated, do not edit it
 */
using System.Collections.Generic;
using xbuffer;

namespace Data
{
    public class GameDataManager
    {
		public static readonly GameDataManager Singleton = new GameDataManager();

        
        private t_AuthorInfoContainer t_AuthorInfoContainer = new t_AuthorInfoContainer();
        

		private GameDataManager()
		{
		
		}

		public void loadAll()
		{
			
			t_AuthorInfoContainer.loadDataFromBin();
			
		}

		
		public List<t_AuthorInfo> Gett_AuthorInfoList()
		{
			return t_AuthorInfoContainer.getList();
		}

		public Dictionary<int, t_AuthorInfo> Gett_AuthorInfoMap()
		{
			return t_AuthorInfoContainer.getMap();
		}
		
	}
}