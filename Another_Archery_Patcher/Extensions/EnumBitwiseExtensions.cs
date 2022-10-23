using System;

namespace Another_Archery_Patcher.Extensions
{
    /// <summary>
    /// Extends <see cref="Enum"/>-based types with support for bitwise operations.<br/>
    /// These methods should <b>only be used on enums with <see cref="FlagsAttribute"/></b>.
    /// </summary>
    public static class EnumBitwiseExtensions
    {
        #region TypeCasting
        /// <summary>
        /// Converts an <see cref="Enum"/> value to <see cref="int"/>.
        /// </summary>
        /// <typeparam name="T"><see cref="Enum"/>-based typename.</typeparam>
        /// <param name="e">Enum value</param>
        /// <returns>The integral representation of <paramref name="e"/></returns>
        public static int ToInt<T>(this T e) where T : Enum => Convert.ToInt32(Convert.ChangeType(e, e.GetTypeCode()));
        /// <summary>
        /// Converts an <see cref="int"/> value to the specified <see cref="Enum"/>-based type.
        /// </summary>
        /// <remarks>
        /// This method should <b>only be used when you know the resulting value will be valid!</b>
        /// </remarks>
        /// <typeparam name="T"><see cref="Enum"/>-based typename.</typeparam>
        /// <param name="i">Integral value</param>
        /// <returns>The enum representation of <paramref name="i"/></returns>
        public static T ToEnum<T>(this int i) where T : Enum => (T)Convert.ChangeType(i, Activator.CreateInstance<T>().GetTypeCode());
        #endregion TypeCasting

        #region Operands
        /// <summary>
        /// Performs a bitwise OR operation on two enum values of an arbitrary type.
        /// </summary>
        /// <remarks>
        /// <b>This method should only be used on enums with <see cref="FlagsAttribute"/></b>.
        /// </remarks>
        /// <typeparam name="T">Any <see cref="Enum"/> type.</typeparam>
        /// <param name="l">The left-side number in the operation.</param>
        /// <param name="r">The right-side number in the operation.</param>
        /// <returns>The result of the operation <b><paramref name="l"/> | <paramref name="r"/></b></returns>
        public static T BitwiseOR<T>(this T l, T r) where T : Enum => (l.ToInt() | r.ToInt()).ToEnum<T>();
        /// <summary>
        /// Performs a bitwise AND operation on two enum values of an arbitrary type.
        /// </summary>
        /// <remarks>
        /// <b>This method should only be used on enums with <see cref="FlagsAttribute"/></b>.
        /// </remarks>
        /// <typeparam name="T">Any <see cref="Enum"/> type.</typeparam>
        /// <param name="l">The left-side number in the operation.</param>
        /// <param name="r">The right-side number in the operation.</param>
        /// <returns>The result of the operation <b><paramref name="l"/> &amp; <paramref name="r"/></b></returns>
        public static T BitwiseAND<T>(this T l, T r) where T : Enum => (l.ToInt() & r.ToInt()).ToEnum<T>();
        /// <summary>
        /// Performs a bitwise XOR operation on two enum values of an arbitrary type.
        /// </summary>
        /// <remarks>
        /// <b>This method should only be used on enums with <see cref="FlagsAttribute"/></b>.
        /// </remarks>
        /// <typeparam name="T">Any <see cref="Enum"/> type.</typeparam>
        /// <param name="l">The left-side number in the operation.</param>
        /// <param name="r">The right-side number in the operation.</param>
        /// <returns>The result of the operation <b><paramref name="l"/> ^ <paramref name="r"/></b></returns>
        public static T BitwiseXOR<T>(this T l, T r) where T : Enum => (l.ToInt() ^ r.ToInt()).ToEnum<T>();
        /// <summary>
        /// Performs a bitwise NOT operation on the given enum value of an arbitrary type.
        /// </summary>
        /// <remarks>
        /// <b>This method should only be used on enums with <see cref="FlagsAttribute"/></b>.
        /// </remarks>
        /// <typeparam name="T">Any <see cref="Enum"/> type.</typeparam>
        /// <param name="l">The number in the operation.</param>
        /// <returns>The result of the operation <b>~<paramref name="l"/></b></returns>
        public static T BitwiseNOT<T>(this T l) where T : Enum => (~l.ToInt()).ToEnum<T>();
        #endregion Operands
    }
}
