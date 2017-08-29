/*
 * Copyright 2012-2017 Scott MacDonald
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using Scott.Forge.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Scott.Forge.Tests.Content
{
    /// <summary>
    ///  Testable asset foo.
    /// </summary>
    public class FooAsset : IEquatable<FooAsset>
    {
        public FooAsset(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public string Value { get; }

        public override bool Equals(object other)
        {
            if (other is FooAsset)
            {
                return Equals(other as FooAsset);
            }

            return false;
        }

        public bool Equals(FooAsset other)
        {
            return Name.Equals(other.Name) && Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        ///  Simple serialziation for test purposes.
        /// </summary>
        public byte[] ToBytes()
        {
            var outStream = new MemoryStream();

            using (var writer = new BinaryWriter(outStream))
            {
                writer.Write(Name);
                writer.Write(Value);
            }

            return outStream.ToArray();
        }

        /// <summary>
        ///  Simple serialziation for test purposes.
        /// </summary>
        public static FooAsset FromBytes(Stream inStream)
        {
            using (var reader = new BinaryReader(inStream))
            {
                var name = reader.ReadString();
                var value = reader.ReadString();

                return new FooAsset(name, value);
            }
        }
    }

    public class FooAssetContentReader : IContentReader<FooAsset>
    {
        public Task<FooAsset> Read(Stream inputStream, string assetPath, IContentManager content)
        {
            return Task.FromResult(FooAsset.FromBytes(inputStream));
        }
    }

    public class BarAsset : IEquatable<BarAsset>, IDisposable
    {
        public BarAsset(string name, int value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public int Value { get; }

        public override bool Equals(object other)
        {
            if (other is BarAsset)
            {
                return Equals(other as BarAsset);
            }

            return false;
        }

        public bool Equals(BarAsset other)
        {
            return Name.Equals(other.Name) && Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        ///  Simple serialziation for test purposes.
        /// </summary>
        public byte[] ToBytes()
        {
            var outStream = new MemoryStream();

            using (var writer = new BinaryWriter(outStream))
            {
                writer.Write(Name);
                writer.Write(Value);
            }

            return outStream.ToArray();
        }

        /// <summary>
        ///  Simple serialziation for test purposes.
        /// </summary>
        public static BarAsset FromBytes(Stream inStream)
        {
            using (var reader = new BinaryReader(inStream))
            {
                var name = reader.ReadString();
                var value = reader.ReadInt32();

                return new BarAsset(name, value);
            }
        }

        #region IDisposable Support
        public bool WasDisposed { get; private set; } = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!WasDisposed)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                WasDisposed = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }

    public class BarAssetContentReader : IContentReader<BarAsset>
    {
        public Task<BarAsset> Read(Stream inputStream, string assetPath, IContentManager content)
        {
            return Task.FromResult(BarAsset.FromBytes(inputStream));
        }
    }

    public class TestableContentContainer : IContentContainer
    {
        public IDictionary<string, byte[]> Assets { get; set; } = new Dictionary<string, byte[]>();

        public TestableContentContainer()
        {
        }

        public void Add(FooAsset asset)
        {
            Assets.Add(ForgeContentManager.NormalizeFilePath(asset.Name), asset.ToBytes());
        }

        public void Add(BarAsset asset)
        {
            Assets.Add(ForgeContentManager.NormalizeFilePath(asset.Name), asset.ToBytes());
        }

        public Task<bool> TryReadItem(string assetName, out Stream readStream)
        {
            if (Assets.ContainsKey(assetName))
            {
                readStream = new MemoryStream(Assets[assetName]);
                return Task.FromResult(true);
            }
            else
            {
                readStream = null;
                return Task.FromResult(false);
            }
        }
    }
}
