using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Vocalia.Editor.Db;

namespace Vocalia.Editor.Repository
{
    public interface IEditorRepository
    {
        Task AddEditAsync(Guid sessionUid, string userUid, Edit edit);

        Task<FileStream> GetEditStreamAsync(Guid sessionUid, string userUid);
    }
}
