﻿using System;
using System.IO;

namespace Ookii.Dialogs.Wpf
{
    /// <summary>
    /// Represents an animation for the <see cref="ProgressDialog"/> loaded from a Win32 resource.
    /// </summary>
    /// <threadsafety instance="false" static="true" />
    public sealed class AnimationResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationResource"/> class.
        /// </summary>
        /// <param name="resourceFile">The file containing the animation resource.</param>
        /// <param name="resourceId">The resource ID of the animation resource.</param>
        /// <exception cref="ArgumentNullException"><paramref name="resourceFile"/> is <see langword="null"/>.</exception>
        public AnimationResource(string resourceFile, int resourceId)
        {
            ResourceFile = resourceFile ?? throw new ArgumentNullException(nameof(resourceFile));
            ResourceId = resourceId;
        }

        /// <summary>
        /// Gets the name of the file containing the animation resource.
        /// </summary>
        /// <value>
        /// The name of the file containing the animation resource. This is typically a DLL or EXE file.
        /// </value>
        public string ResourceFile { get; private set; }

        /// <summary>
        /// Gets the ID of the animation resource.
        /// </summary>
        /// <value>
        /// The ID of the animation resource.
        /// </value>
        public int ResourceId { get; private set; }

        /// <summary>
        /// Gets a default animation from shell32.dll.
        /// </summary>
        /// <param name="animation">The animation to get.</param>
        /// <returns>An instance of the <see cref="AnimationResource"/> class representing the specified animation.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="animation"/> parameter was not a value defined in the
        /// <see cref="ShellAnimation"/> enumeration.</exception>
        public static AnimationResource GetShellAnimation(ShellAnimation animation)
        {
            if (!Enum.IsDefined(animation))
                throw new ArgumentOutOfRangeException(nameof(animation));

            return new AnimationResource("shell32.dll", (int)animation);
        }

        internal SafeModuleHandle LoadLibrary()
        {
            SafeModuleHandle handle = NativeMethods.LoadLibraryEx(ResourceFile, IntPtr.Zero, NativeMethods.LoadLibraryExFlags.LoadLibraryAsDatafile);
            if (handle.IsInvalid)
            {
                int error = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                if (error == NativeMethods.ErrorFileNotFound)
                    throw new FileNotFoundException(string.Format(System.Globalization.CultureInfo.CurrentCulture, Properties.Resources.FileNotFoundFormat, ResourceFile));
                else
                    throw new System.ComponentModel.Win32Exception(error);
            }

            return handle;
        }
    }
}
