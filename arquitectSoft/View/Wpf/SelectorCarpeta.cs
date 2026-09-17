using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace arquitectSoft.View.Wpf
{
    /// <summary>
    /// Elegir carpeta con el cuadro del Explorador de Windows (barra de direcciones, accesos rápidos,
    /// Este equipo…), en lugar del árbol viejo de FolderBrowserDialog. Es el diálogo común de Windows
    /// (IFileOpenDialog con FOS_PICKFOLDERS), sin librerías extra.
    /// </summary>
    public static class SelectorCarpeta
    {
        /// <summary>Devuelve la carpeta elegida o null si se canceló.</summary>
        public static string Elegir(Window owner, string titulo, string inicial)
        {
            IFileOpenDialog dlg = null;
            try
            {
                dlg = (IFileOpenDialog)new FileOpenDialogRCW();
                dlg.GetOptions(out uint opciones);
                dlg.SetOptions(opciones | FOS_PICKFOLDERS | FOS_FORCEFILESYSTEM | FOS_PATHMUSTEXIST);
                if (!string.IsNullOrEmpty(titulo)) dlg.SetTitle(titulo);
                dlg.SetOkButtonLabel("Guardar aquí");

                if (!string.IsNullOrEmpty(inicial) && System.IO.Directory.Exists(inicial))
                {
                    Guid iid = typeof(IShellItem).GUID;
                    if (SHCreateItemFromParsingName(inicial, IntPtr.Zero, ref iid, out IShellItem item) == 0)
                        dlg.SetFolder(item);
                }

                IntPtr hwnd = owner != null ? new WindowInteropHelper(owner).Handle : IntPtr.Zero;
                int hr = dlg.Show(hwnd);
                if (hr != 0) return null;   // cancelado (0x800704C7) u otro error

                dlg.GetResult(out IShellItem resultado);
                resultado.GetDisplayName(SIGDN_FILESYSPATH, out IntPtr ptr);
                try { return Marshal.PtrToStringUni(ptr); }
                finally { Marshal.FreeCoTaskMem(ptr); }
            }
            catch (COMException)
            {
                // Si el diálogo moderno no está disponible, el de siempre.
                using (var fb = new System.Windows.Forms.FolderBrowserDialog())
                {
                    if (!string.IsNullOrEmpty(inicial)) fb.SelectedPath = inicial;
                    return fb.ShowDialog() == System.Windows.Forms.DialogResult.OK ? fb.SelectedPath : null;
                }
            }
            finally
            {
                if (dlg != null) Marshal.ReleaseComObject(dlg);
            }
        }

        private const uint FOS_PICKFOLDERS = 0x20;
        private const uint FOS_FORCEFILESYSTEM = 0x40;
        private const uint FOS_PATHMUSTEXIST = 0x800;
        private const uint SIGDN_FILESYSPATH = 0x80058000;

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, PreserveSig = true)]
        private static extern int SHCreateItemFromParsingName(string pszPath, IntPtr pbc, ref Guid riid,
            [MarshalAs(UnmanagedType.Interface)] out IShellItem ppv);

        [ComImport, Guid("DC1C5A9C-E88A-4dde-A5A1-60F82A20AEF7")]
        private class FileOpenDialogRCW { }

        [ComImport, Guid("d57c7288-d4ad-4768-be02-9d969532d960"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IFileOpenDialog
        {
            [PreserveSig] int Show(IntPtr parent);
            void SetFileTypes(uint cFileTypes, IntPtr rgFilterSpec);
            void SetFileTypeIndex(uint iFileType);
            void GetFileTypeIndex(out uint piFileType);
            void Advise(IntPtr pfde, out uint pdwCookie);
            void Unadvise(uint dwCookie);
            void SetOptions(uint fos);
            void GetOptions(out uint pfos);
            void SetDefaultFolder(IShellItem psi);
            void SetFolder(IShellItem psi);
            void GetFolder(out IShellItem ppsi);
            void GetCurrentSelection(out IShellItem ppsi);
            void SetFileName([MarshalAs(UnmanagedType.LPWStr)] string pszName);
            void GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);
            void SetTitle([MarshalAs(UnmanagedType.LPWStr)] string pszTitle);
            void SetOkButtonLabel([MarshalAs(UnmanagedType.LPWStr)] string pszText);
            void SetFileNameLabel([MarshalAs(UnmanagedType.LPWStr)] string pszLabel);
            void GetResult(out IShellItem ppsi);
            void AddPlace(IShellItem psi, int fdap);
            void SetDefaultExtension([MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);
            void Close(int hr);
            void SetClientGuid(ref Guid guid);
            void ClearClientData();
            void SetFilter(IntPtr pFilter);
            void GetResults(out IntPtr ppenum);
            void GetSelectedItems(out IntPtr ppsai);
        }

        [ComImport, Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IShellItem
        {
            void BindToHandler(IntPtr pbc, ref Guid bhid, ref Guid riid, out IntPtr ppv);
            void GetParent(out IShellItem ppsi);
            void GetDisplayName(uint sigdnName, out IntPtr ppszName);
            void GetAttributes(uint sfgaoMask, out uint psfgaoAttribs);
            void Compare(IShellItem psi, uint hint, out int piOrder);
        }
    }
}
