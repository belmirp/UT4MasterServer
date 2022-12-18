﻿using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UT4MasterServer
{
	public class Token
	{
		[BsonElement("Value")]
		public string Value { get; set; }

		[BsonElement("Expiration")]
		public DateTime Expiration { get; set; }

		[BsonIgnore]
		public int ExpirySeconds { get => (int)(Expiration - DateTime.UtcNow).TotalSeconds; }

		[BsonIgnore]
		public bool IsExpired { get => DateTime.UtcNow > Expiration; }

		public Token(string value, DateTime expiration)
		{
			Value = value;
			Expiration = expiration;
		}

		public override string ToString()
		{
			return Value;
		}



		public static Token Generate(TimeSpan expirationDuration)
		{
			return Generate(DateTime.UtcNow + expirationDuration);
		}

		public static Token Generate(DateTime expirationTime)
		{
			// NOTE: epic uses sometimes uses JWT and sometimes just random 16-byte hexstring
			//       depending on the token type

			// we could infer importance of length/security based on expiration

			Random random = new Random();
			byte[] bytes = new byte[32];
			random.NextBytes(bytes);

			var tokenString = Convert.ToHexString(bytes);

			return new Token(tokenString, expirationTime);
		}
	}
}
