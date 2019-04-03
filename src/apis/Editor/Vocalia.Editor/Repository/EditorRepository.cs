using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Vocalia.Editor.Db;

namespace Vocalia.Editor.Repository
{
    public class EditorRepository : IEditorRepository
    {
        public Task AddEditAsync(Guid sessionUid, string userUid, Edit edit)
        {
            throw new NotImplementedException();
        }

        public Task<FileStream> GetEditStreamAsync(Guid sessionUid, string userUid)
        {
            throw new NotImplementedException();
        }
    }
}
