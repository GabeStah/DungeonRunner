using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.Hearthstone.Entities;

namespace DungeonRunner
{
	internal class EntityCardComparitor : IEqualityComparer<Entity>
	{
		private readonly Func<Entity, Entity, bool> _lambdaComparer;
		private readonly Func<Entity, int> _lambdaHash;

		public EntityCardComparitor(Func<Entity, Entity, bool> lambdaComparer) :
			this(lambdaComparer, o => 0)
		{
		}

		public EntityCardComparitor(Func<Entity, Entity, bool> lambdaComparer, Func<Entity, int> lambdaHash)
		{
			_lambdaComparer = lambdaComparer ?? throw new ArgumentNullException("lambdaComparer");
			_lambdaHash = lambdaHash ?? throw new ArgumentNullException("lambdaHash");
		}

		public bool Equals(Entity x, Entity y)
		{
			return _lambdaComparer(x, y);
		}

		public int GetHashCode(Entity obj)
		{
			return _lambdaHash(obj);
		}
	}
}
