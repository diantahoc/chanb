#If TripCodeSupport Then
Option Strict Off
Imports Microsoft.VisualBasic
Imports System
Imports System.Linq
Imports System.Text

'// 

''' <summary>
''' Provides the Unix crypt() encryption algorithm.
''' </summary>
''' <remarks>
''' This class is a port from Java source. I do not understand the underlying algorithms, I just converted it to C# and it works.
''' Because I do not understand the underlying algorithms I cannot give most of the variables useful names. I have no clue what their
''' significance is. I tried to give the variable names as much meaning as possible, but the original source just called them a, b, c , etc...
''' 
''' A very important thing to note is that all ints in this code are UNSIGNED ints! Do not change this, ever!!! It will seriously fuckup the working
''' of this class. It uses major bitshifting and while Java gives you the >>> operator to signify a right bitshift WITHOUT setting the MSB for
''' a signed int, C# does not have this operator and will just set the new MSB for you if it happened to be set the moment you bitshifted it.
''' This is undesirable for most bitshifts and in the cases it did matter, I casted the variable back to an int. This was only required where
''' a variable was on the right-side of a bitshift operator.
''' 
''' Reconverted to VB using Instant VB
''' </remarks>
Friend NotInheritable Class UnixCrypt

    Private Const m_encryptionSaltCharacters As String = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789./"

    Private Shared ReadOnly m_saltTranslation() As UInteger = {&H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H1, &H2, &H3, &H4, &H5, &H6, &H7, &H8, &H9, &HA, &HB, &H5, &H6, &H7, &H8, &H9, &HA, &HB, &HC, &HD, &HE, &HF, &H10, &H11, &H12, &H13, &H14, &H15, &H16, &H17, &H18, &H19, &H1A, &H1B, &H1C, &H1D, &H1E, &H1F, &H20, &H21, &H22, &H23, &H24, &H25, &H20, &H21, &H22, &H23, &H24, &H25, &H26, &H27, &H28, &H29, &H2A, &H2B, &H2C, &H2D, &H2E, &H2F, &H30, &H31, &H32, &H33, &H34, &H35, &H36, &H37, &H38, &H39, &H3A, &H3B, &H3C, &H3D, &H3E, &H3F, &H0, &H0, &H0, &H0, &H0}

    Private Shared ReadOnly m_shifts() As Boolean = {False, False, True, True, True, True, True, True, False, True, True, True, True, True, True, False}

    Private Shared ReadOnly m_skb(,) As UInteger = {{&H0, &H10, &H20000000, &H20000010, &H10000, &H10010, &H20010000, &H20010010, &H800, &H810, &H20000800, &H20000810, &H10800, &H10810, &H20010800, &H20010810, &H20, &H30, &H20000020, &H20000030, &H10020, &H10030, &H20010020, &H20010030, &H820, &H830, &H20000820, &H20000830, &H10820, &H10830, &H20010820, &H20010830, &H80000, &H80010, &H20080000, &H20080010, &H90000, &H90010, &H20090000, &H20090010, &H80800, &H80810, &H20080800, &H20080810, &H90800, &H90810, &H20090800, &H20090810, &H80020, &H80030, &H20080020, &H20080030, &H90020, &H90030, &H20090020, &H20090030, &H80820, &H80830, &H20080820, &H20080830, &H90820, &H90830, &H20090820, &H20090830}, {&H0, &H2000000, &H2000, &H2002000, &H200000, &H2200000, &H202000, &H2202000, &H4, &H2000004, &H2004, &H2002004, &H200004, &H2200004, &H202004, &H2202004, &H400, &H2000400, &H2400, &H2002400, &H200400, &H2200400, &H202400, &H2202400, &H404, &H2000404, &H2404, &H2002404, &H200404, &H2200404, &H202404, &H2202404, &H10000000, &H12000000, &H10002000, &H12002000, &H10200000, &H12200000, &H10202000, &H12202000, &H10000004, &H12000004, &H10002004, &H12002004, &H10200004, &H12200004, &H10202004, &H12202004, &H10000400, &H12000400, &H10002400, &H12002400, &H10200400, &H12200400, &H10202400, &H12202400, &H10000404, &H12000404, &H10002404, &H12002404, &H10200404, &H12200404, &H10202404, &H12202404}, {&H0, &H1, &H40000, &H40001, &H1000000, &H1000001, &H1040000, &H1040001, &H2, &H3, &H40002, &H40003, &H1000002, &H1000003, &H1040002, &H1040003, &H200, &H201, &H40200, &H40201, &H1000200, &H1000201, &H1040200, &H1040201, &H202, &H203, &H40202, &H40203, &H1000202, &H1000203, &H1040202, &H1040203, &H8000000, &H8000001, &H8040000, &H8040001, &H9000000, &H9000001, &H9040000, &H9040001, &H8000002, &H8000003, &H8040002, &H8040003, &H9000002, &H9000003, &H9040002, &H9040003, &H8000200, &H8000201, &H8040200, &H8040201, &H9000200, &H9000201, &H9040200, &H9040201, &H8000202, &H8000203, &H8040202, &H8040203, &H9000202, &H9000203, &H9040202, &H9040203}, {&H0, &H100000, &H100, &H100100, &H8, &H100008, &H108, &H100108, &H1000, &H101000, &H1100, &H101100, &H1008, &H101008, &H1108, &H101108, &H4000000, &H4100000, &H4000100, &H4100100, &H4000008, &H4100008, &H4000108, &H4100108, &H4001000, &H4101000, &H4001100, &H4101100, &H4001008, &H4101008, &H4001108, &H4101108, &H20000, &H120000, &H20100, &H120100, &H20008, &H120008, &H20108, &H120108, &H21000, &H121000, &H21100, &H121100, &H21008, &H121008, &H21108, &H121108, &H4020000, &H4120000, &H4020100, &H4120100, &H4020008, &H4120008, &H4020108, &H4120108, &H4021000, &H4121000, &H4021100, &H4121100, &H4021008, &H4121008, &H4021108, &H4121108}, {&H0, &H10000000, &H10000, &H10010000, &H4, &H10000004, &H10004, &H10010004, &H20000000, &H30000000, &H20010000, &H30010000, &H20000004, &H30000004, &H20010004, &H30010004, &H100000, &H10100000, &H110000, &H10110000, &H100004, &H10100004, &H110004, &H10110004, &H20100000, &H30100000, &H20110000, &H30110000, &H20100004, &H30100004, &H20110004, &H30110004, &H1000, &H10001000, &H11000, &H10011000, &H1004, &H10001004, &H11004, &H10011004, &H20001000, &H30001000, &H20011000, &H30011000, &H20001004, &H30001004, &H20011004, &H30011004, &H101000, &H10101000, &H111000, &H10111000, &H101004, &H10101004, &H111004, &H10111004, &H20101000, &H30101000, &H20111000, &H30111000, &H20101004, &H30101004, &H20111004, &H30111004}, {&H0, &H8000000, &H8, &H8000008, &H400, &H8000400, &H408, &H8000408, &H20000, &H8020000, &H20008, &H8020008, &H20400, &H8020400, &H20408, &H8020408, &H1, &H8000001, &H9, &H8000009, &H401, &H8000401, &H409, &H8000409, &H20001, &H8020001, &H20009, &H8020009, &H20401, &H8020401, &H20409, &H8020409, &H2000000, &HA000000, &H2000008, &HA000008, &H2000400, &HA000400, &H2000408, &HA000408, &H2020000, &HA020000, &H2020008, &HA020008, &H2020400, &HA020400, &H2020408, &HA020408, &H2000001, &HA000001, &H2000009, &HA000009, &H2000401, &HA000401, &H2000409, &HA000409, &H2020001, &HA020001, &H2020009, &HA020009, &H2020401, &HA020401, &H2020409, &HA020409}, {&H0, &H100, &H80000, &H80100, &H1000000, &H1000100, &H1080000, &H1080100, &H10, &H110, &H80010, &H80110, &H1000010, &H1000110, &H1080010, &H1080110, &H200000, &H200100, &H280000, &H280100, &H1200000, &H1200100, &H1280000, &H1280100, &H200010, &H200110, &H280010, &H280110, &H1200010, &H1200110, &H1280010, &H1280110, &H200, &H300, &H80200, &H80300, &H1000200, &H1000300, &H1080200, &H1080300, &H210, &H310, &H80210, &H80310, &H1000210, &H1000310, &H1080210, &H1080310, &H200200, &H200300, &H280200, &H280300, &H1200200, &H1200300, &H1280200, &H1280300, &H200210, &H200310, &H280210, &H280310, &H1200210, &H1200310, &H1280210, &H1280310}, {&H0, &H4000000, &H40000, &H4040000, &H2, &H4000002, &H40002, &H4040002, &H2000, &H4002000, &H42000, &H4042000, &H2002, &H4002002, &H42002, &H4042002, &H20, &H4000020, &H40020, &H4040020, &H22, &H4000022, &H40022, &H4040022, &H2020, &H4002020, &H42020, &H4042020, &H2022, &H4002022, &H42022, &H4042022, &H800, &H4000800, &H40800, &H4040800, &H802, &H4000802, &H40802, &H4040802, &H2800, &H4002800, &H42800, &H4042800, &H2802, &H4002802, &H42802, &H4042802, &H820, &H4000820, &H40820, &H4040820, &H822, &H4000822, &H40822, &H4040822, &H2820, &H4002820, &H42820, &H4042820, &H2822, &H4002822, &H42822, &H4042822}}

    Private Shared ReadOnly m_SPTranslationTable(,) As UInteger = {{&H820200, &H20000, &H80800000L, &H80820200L, &H800000, &H80020200L, &H80020000L, &H80800000L, &H80020200L, &H820200, &H820000, &H80000200L, &H80800200L, &H800000, &H0, &H80020000L, &H20000, &H80000000L, &H800200, &H20200, &H80820200L, &H820000, &H80000200L, &H800200, &H80000000L, &H200, &H20200, &H80820000L, &H200, &H80800200L, &H80820000L, &H0, &H0, &H80820200L, &H800200, &H80020000L, &H820200, &H20000, &H80000200L, &H800200, &H80820000L, &H200, &H20200, &H80800000L, &H80020200L, &H80000000L, &H80800000L, &H820000, &H80820200L, &H20200, &H820000, &H80800200L, &H800000, &H80000200L, &H80020000L, &H0, &H20000, &H800000, &H80800200L, &H820200, &H80000000L, &H80820000L, &H200, &H80020200L}, {&H10042004, &H0, &H42000, &H10040000, &H10000004, &H2004, &H10002000, &H42000, &H2000, &H10040004, &H4, &H10002000, &H40004, &H10042000, &H10040000, &H4, &H40000, &H10002004, &H10040004, &H2000, &H42004, &H10000000, &H0, &H40004, &H10002004, &H42004, &H10042000, &H10000004, &H10000000, &H40000, &H2004, &H10042004, &H40004, &H10042000, &H10002000, &H42004, &H10042004, &H40004, &H10000004, &H0, &H10000000, &H2004, &H40000, &H10040004, &H2000, &H10000000, &H42004, &H10002004, &H10042000, &H2000, &H0, &H10000004, &H4, &H10042004, &H42000, &H10040000, &H10040004, &H40000, &H2004, &H10002000, &H10002004, &H4, &H10040000, &H42000}, {&H41000000, &H1010040, &H40, &H41000040, &H40010000, &H1000000, &H41000040, &H10040, &H1000040, &H10000, &H1010000, &H40000000, &H41010040, &H40000040, &H40000000, &H41010000, &H0, &H40010000, &H1010040, &H40, &H40000040, &H41010040, &H10000, &H41000000, &H41010000, &H1000040, &H40010040, &H1010000, &H10040, &H0, &H1000000, &H40010040, &H1010040, &H40, &H40000000, &H10000, &H40000040, &H40010000, &H1010000, &H41000040, &H0, &H1010040, &H10040, &H41010000, &H40010000, &H1000000, &H41010040, &H40000000, &H40010040, &H41000000, &H1000000, &H41010040, &H10000, &H1000040, &H41000040, &H10040, &H1000040, &H0, &H41010000, &H40000040, &H41000000, &H40010040, &H40, &H1010000}, {&H100402, &H4000400, &H2, &H4100402, &H0, &H4100000, &H4000402, &H100002, &H4100400, &H4000002, &H4000000, &H402, &H4000002, &H100402, &H100000, &H4000000, &H4100002, &H100400, &H400, &H2, &H100400, &H4000402, &H4100000, &H400, &H402, &H0, &H100002, &H4100400, &H4000400, &H4100002, &H4100402, &H100000, &H4100002, &H402, &H100000, &H4000002, &H100400, &H4000400, &H2, &H4100000, &H4000402, &H0, &H400, &H100002, &H0, &H4100002, &H4100400, &H400, &H4000000, &H4100402, &H100402, &H100000, &H4100402, &H2, &H4000400, &H100402, &H100002, &H100400, &H4100000, &H4000402, &H402, &H4000000, &H4000002, &H4100400}, {&H2000000, &H4000, &H100, &H2004108, &H2004008, &H2000100, &H4108, &H2004000, &H4000, &H8, &H2000008, &H4100, &H2000108, &H2004008, &H2004100, &H0, &H4100, &H2000000, &H4008, &H108, &H2000100, &H4108, &H0, &H2000008, &H8, &H2000108, &H2004108, &H4008, &H2004000, &H100, &H108, &H2004100, &H2004100, &H2000108, &H4008, &H2004000, &H4000, &H8, &H2000008, &H2000100, &H2000000, &H4100, &H2004108, &H0, &H4108, &H2000000, &H100, &H4008, &H2000108, &H100, &H0, &H2004108, &H2004008, &H2004100, &H108, &H4000, &H4100, &H2004008, &H2000100, &H108, &H8, &H4108, &H2004000, &H2000008}, {&H20000010, &H80010, &H0, &H20080800, &H80010, &H800, &H20000810, &H80000, &H810, &H20080810, &H80800, &H20000000, &H20000800, &H20000010, &H20080000, &H80810, &H80000, &H20000810, &H20080010, &H0, &H800, &H10, &H20080800, &H20080010, &H20080810, &H20080000, &H20000000, &H810, &H10, &H80800, &H80810, &H20000800, &H810, &H20000000, &H20000800, &H80810, &H20080800, &H80010, &H0, &H20000800, &H20000000, &H800, &H20080010, &H80000, &H80010, &H20080810, &H80800, &H10, &H20080810, &H80800, &H80000, &H20000810, &H20000010, &H20080000, &H80810, &H0, &H800, &H20000010, &H20000810, &H20080800, &H20080000, &H810, &H10, &H20080010}, {&H1000, &H80, &H400080, &H400001, &H401081, &H1001, &H1080, &H0, &H400000, &H400081, &H81, &H401000, &H1, &H401080, &H401000, &H81, &H400081, &H1000, &H1001, &H401081, &H0, &H400080, &H400001, &H1080, &H401001, &H1081, &H401080, &H1, &H1081, &H401001, &H80, &H400000, &H1081, &H401000, &H401001, &H81, &H1000, &H80, &H400000, &H401001, &H400081, &H1081, &H1080, &H0, &H80, &H400001, &H1, &H400080, &H0, &H400081, &H400080, &H1080, &H81, &H1000, &H401081, &H400000, &H401080, &H1, &H1001, &H401081, &H400001, &H401080, &H401000, &H1001}, {&H8200020, &H8208000, &H8020, &H0, &H8008000, &H200020, &H8200000, &H8208020, &H20, &H8000000, &H208000, &H8020, &H208020, &H8008020, &H8000020, &H8200000, &H8000, &H208020, &H200020, &H8008000, &H8208020, &H8000020, &H0, &H208000, &H8000000, &H200000, &H8008020, &H8200020, &H200000, &H8000, &H8208000, &H20, &H200000, &H8000, &H8000020, &H8208020, &H8020, &H8000000, &H0, &H208000, &H8200020, &H8008020, &H8008000, &H200020, &H8208000, &H20, &H200020, &H8008000, &H8208020, &H200000, &H8200000, &H8000020, &H208000, &H8020, &H8008020, &H8200000, &H20, &H8208000, &H208020, &H0, &H8000000, &H8200020, &H8000, &H208020}}

    Private Shared ReadOnly m_characterConversionTable() As UInteger = {&H2E, &H2F, &H30, &H31, &H32, &H33, &H34, &H35, &H36, &H37, &H38, &H39, &H41, &H42, &H43, &H44, &H45, &H46, &H47, &H48, &H49, &H4A, &H4B, &H4C, &H4D, &H4E, &H4F, &H50, &H51, &H52, &H53, &H54, &H55, &H56, &H57, &H58, &H59, &H5A, &H61, &H62, &H63, &H64, &H65, &H66, &H67, &H68, &H69, &H6A, &H6B, &H6C, &H6D, &H6E, &H6F, &H70, &H71, &H72, &H73, &H74, &H75, &H76, &H77, &H78, &H79, &H7A}

    Private Const m_desIterations As Integer = 16

    Private Sub New()
    End Sub
    Private Shared Function FourBytesToInt(ByVal inputBytes() As Byte, ByVal offset As UInteger) As UInteger
        ' I used an int here because the compiler would complain the stuff below would require a cast from int to uint.
        ' To keep the code cleaner I opted to use an int and cast it when I returned it.
        Dim resultValue As Integer = 0

        resultValue = (inputBytes(offset) And &HFF)
        offset += 1
        resultValue = resultValue Or (inputBytes(offset) And &HFF) << 8
        offset += 1
        resultValue = resultValue Or (inputBytes(offset) And &HFF) << 16
        offset += 1
        resultValue = resultValue Or (inputBytes(offset) And &HFF) << 24
        offset += 1

        Return CUInt(resultValue)
    End Function



    ''' <summary>
    ''' Converts an uint into 4 seperate bytes.
    ''' </summary>
    ''' <param name="inputInt">The uint to convert.</param>
    ''' <param name="outputBytes">The byte buffer into which to store the result.</param>
    ''' <param name="offset">The offset to start storing at in the outputBytes buffer.</param>
    Private Shared Sub IntToFourBytes(ByVal inputInt As UInteger, ByVal outputBytes() As Byte, ByVal offset As UInteger)
        outputBytes(offset) = CByte(inputInt And &HFF)
        offset += 1
        outputBytes(offset) = CByte((inputInt >> 8) And &HFF)
        offset += 1
        outputBytes(offset) = CByte((inputInt >> 16) And &HFF)
        offset += 1
        outputBytes(offset) = CByte((inputInt >> 24) And &HFF)
        offset += 1
    End Sub



    ''' <summary>
    ''' Performs some operation on 4 uints. It's labeled PERM_OP in the original source.
    ''' </summary>
    ''' <param name="firstInt">The first uint to use.</param>
    ''' <param name="secondInt">The second uint to use.</param>
    ''' <param name="thirdInt">The third uint to use.</param>
    ''' <param name="fourthInt">The fourth uint to use.</param>
    ''' <param name="operationResults">An array of 2 uints that are the result of this operation.</param>
    Private Shared Sub PermOperation(ByVal firstInt As UInteger, ByVal secondInt As UInteger, ByVal thirdInt As UInteger, ByVal fourthInt As UInteger, ByVal operationResults() As UInteger)
        ' Because here an uint variable is at the right side of a bitshift, I needed to cast it to int. See the remarks of the class itself
        ' for more details.
        Dim tempInt As UInteger = ((firstInt >> CInt(Fix(thirdInt))) Xor secondInt) And fourthInt
        firstInt = firstInt Xor tempInt << CInt(Fix(thirdInt))
        secondInt = secondInt Xor tempInt

        operationResults(0) = firstInt
        operationResults(1) = secondInt
    End Sub



    ''' <summary>
    ''' Performs some operation on 3 uints. It's labeled HPERM_OP in the original source.
    ''' </summary>
    ''' <param name="firstInt">The first uint to use.</param>
    ''' <param name="secondInt">The second int to use.</param>
    ''' <param name="thirdInt">The third uint to use.</param>
    ''' <returns>An int that is the result of this operation.</returns>
    Private Shared Function HPermOperation(ByVal firstInt As UInteger, ByVal secondInt As Integer, ByVal thirdInt As UInteger) As UInteger
        ' The variable secondInt is always used to calculate the number at the right side of a
        ' bitshift. It is not used anywhere else, so I made the method parameter an int, to avoid
        ' unnecessary casting.
        Dim tempInt As UInteger = ((firstInt << (16 - secondInt)) Xor firstInt) And thirdInt
        Dim returnInt As UInteger = firstInt Xor tempInt Xor (tempInt >> (16 - secondInt))

        Return returnInt
    End Function



    ''' <summary>
    ''' This method does some very complex bit manipulations.
    ''' </summary>
    ''' <param name="encryptionKey">The input data to use for the bit manipulations.</param>
    ''' <returns>m_desIterations * 2 number of uints that are the result of the manipulations.</returns>
    Private Shared Function SetDESKey(ByVal encryptionKey() As Byte) As UInteger()
        Dim schedule(m_desIterations * 2 - 1) As UInteger

        Dim firstInt As UInteger = FourBytesToInt(encryptionKey, 0)
        Dim secondInt As UInteger = FourBytesToInt(encryptionKey, 4)

        Dim operationResults(1) As UInteger
        PermOperation(secondInt, firstInt, 4, &HF0F0F0F, operationResults)
        secondInt = operationResults(0)
        firstInt = operationResults(1)

        firstInt = HPermOperation(firstInt, -2, &HCCCC0000L)
        secondInt = HPermOperation(secondInt, -2, &HCCCC0000L)

        PermOperation(secondInt, firstInt, 1, &H55555555, operationResults)
        secondInt = operationResults(0)
        firstInt = operationResults(1)

        PermOperation(firstInt, secondInt, 8, &HFF00FF, operationResults)
        firstInt = operationResults(0)
        secondInt = operationResults(1)

        PermOperation(secondInt, firstInt, 1, &H55555555, operationResults)
        secondInt = operationResults(0)
        firstInt = operationResults(1)

        secondInt = (((secondInt And &HFF) << 16) Or (secondInt And &HFF00) Or ((secondInt And &HFF0000) >> 16) Or ((firstInt And &HF0000000L) >> 4))

        firstInt = firstInt And &HFFFFFFF

        Dim needToShift As Boolean
        Dim firstSkbValue, secondSkbValue As UInteger
        Dim scheduleIndex As UInteger = 0

        For index As Integer = 0 To m_desIterations - 1
            needToShift = m_shifts(index)
            If needToShift Then
                firstInt = (firstInt >> 2) Or (firstInt << 26)
                secondInt = (secondInt >> 2) Or (secondInt << 26)
            Else
                firstInt = (firstInt >> 1) Or (firstInt << 27)
                secondInt = (secondInt >> 1) Or (secondInt << 27)
            End If

            firstInt = firstInt And &HFFFFFFF
            secondInt = secondInt And &HFFFFFFF

            firstSkbValue = m_skb(0, firstInt And &H3F) Or m_skb(1, ((firstInt >> 6) And &H3) Or ((firstInt >> 7) And &H3C)) Or m_skb(2, ((firstInt >> 13) And &HF) Or ((firstInt >> 14) And &H30)) Or m_skb(3, ((firstInt >> 20) And &H1) Or ((firstInt >> 21) And &H6) Or ((firstInt >> 22) And &H38))

            secondSkbValue = m_skb(4, secondInt And &H3F) Or m_skb(5, ((secondInt >> 7) And &H3) Or ((secondInt >> 8) And &H3C)) Or m_skb(6, (secondInt >> 15) And &H3F) Or m_skb(7, ((secondInt >> 21) And &HF) Or ((secondInt >> 22) And &H30))

            schedule(scheduleIndex) = ((secondSkbValue << 16) Or (firstSkbValue And &HFFFF)) And &HFFFFFFFFL
            scheduleIndex += 1
            firstSkbValue = ((firstSkbValue >> 16) Or (secondSkbValue And &HFFFF0000L))

            firstSkbValue = (firstSkbValue << 4) Or (firstSkbValue >> 28)
            schedule(scheduleIndex) = firstSkbValue And &HFFFFFFFFL
            scheduleIndex += 1
        Next index

        Return schedule
    End Function



    ''' <summary>
    ''' This method does some bit manipulations.
    ''' </summary>
    ''' <param name="left">An input that is manipulated and then used for output.</param>
    ''' <param name="right">This is used for the bit manipulation.</param>
    ''' <param name="scheduleIndex">The index of an uint to use from the schedule array.</param>
    ''' <param name="firstSaltTranslator">The translated salt for the first salt character.</param>
    ''' <param name="secondSaltTranslator">The translated salt for the second salt character.</param>
    ''' <param name="schedule">The schedule arrray calculated before.</param>
    ''' <returns>The result of these manipulations.</returns>
    Private Shared Function DEncrypt(ByVal left As UInteger, ByVal right As UInteger, ByVal scheduleIndex As UInteger, ByVal firstSaltTranslator As UInteger, ByVal secondSaltTranslator As UInteger, ByVal schedule() As UInteger) As UInteger
        Dim firstInt, secondInt, thirdInt As UInteger

        thirdInt = right Xor (right >> 16)
        secondInt = thirdInt And firstSaltTranslator
        thirdInt = thirdInt And secondSaltTranslator

        secondInt = (secondInt Xor (secondInt << 16)) Xor right Xor schedule(scheduleIndex)
        firstInt = (thirdInt Xor (thirdInt << 16)) Xor right Xor schedule(scheduleIndex + 1)
        firstInt = (firstInt >> 4) Or (firstInt << 28)

        left = left Xor (m_SPTranslationTable(1, firstInt And &H3F) Or m_SPTranslationTable(3, (firstInt >> 8) And &H3F) Or m_SPTranslationTable(5, (firstInt >> 16) And &H3F) Or m_SPTranslationTable(7, (firstInt >> 24) And &H3F) Or m_SPTranslationTable(0, secondInt And &H3F) Or m_SPTranslationTable(2, (secondInt >> 8) And &H3F) Or m_SPTranslationTable(4, (secondInt >> 16) And &H3F) Or m_SPTranslationTable(6, (secondInt >> 24) And &H3F))

        Return left
    End Function



    ''' <summary>
    ''' Calculates two uints that are used to encrypt the password.
    ''' </summary>
    ''' <param name="schedule">The schedule table calculated earlier.</param>
    ''' <param name="firstSaltTranslator">The first translated salt character.</param>
    ''' <param name="secondSaltTranslator">The second translated salt character.</param>
    ''' <returns>2 uints in an array.</returns>
    Private Shared Function Body(ByVal schedule() As UInteger, ByVal firstSaltTranslator As UInteger, ByVal secondSaltTranslator As UInteger) As UInteger()
        Dim left As UInteger = 0
        Dim right As UInteger = 0
        Dim tempInt As UInteger

        For index As Integer = 0 To 24
            For secondIndex As UInteger = 0 To m_desIterations * 2 - 1 Step 4
                left = DEncrypt(left, right, secondIndex, firstSaltTranslator, secondSaltTranslator, schedule)
                right = DEncrypt(right, left, secondIndex + 2, firstSaltTranslator, secondSaltTranslator, schedule)
            Next secondIndex

            tempInt = left
            left = right
            right = tempInt
        Next index

        tempInt = right
        right = (left >> 1) Or (left << 31)
        left = (tempInt >> 1) Or (tempInt << 31)

        left = left And &HFFFFFFFFL
        right = right And &HFFFFFFFFL

        Dim operationResults(1) As UInteger

        PermOperation(right, left, 1, &H55555555, operationResults)
        right = operationResults(0)
        left = operationResults(1)

        PermOperation(left, right, 8, &HFF00FF, operationResults)
        left = operationResults(0)
        right = operationResults(1)

        PermOperation(right, left, 2, &H33333333, operationResults)
        right = operationResults(0)
        left = operationResults(1)

        PermOperation(left, right, 16, &HFFFF, operationResults)
        left = operationResults(0)
        right = operationResults(1)

        PermOperation(right, left, 4, &HF0F0F0F, operationResults)
        right = operationResults(0)
        left = operationResults(1)

        Dim singleOutputKey(1) As UInteger
        singleOutputKey(0) = left
        singleOutputKey(1) = right

        Return singleOutputKey
    End Function


    ''' <summary>
    ''' Automatically generate the encryption salt (2 random printable characters for use in the encryption) and call the Crypt() method.
    ''' </summary>
    ''' <param name="textToEncrypt">The text that must be encrypted.</param>
    ''' <returns>The encrypted text.</returns>
    Public Shared Function Crypt(ByVal textToEncrypt As String) As String
        Dim randomGenerator As New Random()

        Dim maxGeneratedNumber As Integer = m_encryptionSaltCharacters.Length
        Dim randomIndex As Integer
        Dim encryptionSaltBuilder As New StringBuilder()
        For index As Integer = 0 To 1
            randomIndex = randomGenerator.Next(maxGeneratedNumber)
            encryptionSaltBuilder.Append(m_encryptionSaltCharacters.Chars(randomIndex))
        Next index

        Dim encryptionSalt As String = encryptionSaltBuilder.ToString()
        Dim encryptedString As String = Crypt(encryptionSalt, textToEncrypt)

        Return encryptedString
    End Function



    ''' <summary>
    ''' Encrypts the specified string using the Unix crypt algorithm.
    ''' </summary>
    ''' <param name="encryptionSalt">2 random printable characters that are used to randomize the encryption.</param>
    ''' <param name="textToEncrypt">The text that must be encrypted.</param>
    ''' <returns>The encrypted text.</returns>
    Public Shared Function Crypt(ByVal encryptionSalt As String, ByVal textToEncrypt As String) As String
        If encryptionSalt Is Nothing Then
            Throw New ArgumentNullException("encryptionSalt")
        End If
        If textToEncrypt Is Nothing Then
            Throw New ArgumentNullException("textToEncrypt")
        End If

        Dim isSaltTooSmall As Boolean = (encryptionSalt.Length < 2)
        If isSaltTooSmall Then
            Throw New ArgumentException("The encryptionSalt must be 2 characters big.")
        End If

        Dim firstSaltCharacter As Char = encryptionSalt.Chars(0)
        Dim secondSaltCharacter As Char = encryptionSalt.Chars(1)

        ' Make sure the string builder is big enough AND filled with 13 characters (the length of the encrypted password).
        ' We will use the index operator to set them, but when the characters are not present, even though the string builder
        ' has enough capacity, it will throw an exception.
        Dim encryptionBuffer As New StringBuilder("*************")
        encryptionBuffer(0) = firstSaltCharacter
        encryptionBuffer(1) = secondSaltCharacter

        ' Use the ASCII value of the salt characters to lookup a number in the salt translation table.
        Dim firstSaltTranslator As UInteger = m_saltTranslation(Convert.ToUInt32(firstSaltCharacter))
        Dim secondSaltTranslator As UInteger = m_saltTranslation(Convert.ToUInt32(secondSaltCharacter)) << 4

        ' Build the first encryption key table by taking the ASCII value of every character in the text to encrypt and
        ' multiplying it by two. Note how the cast will not lose any information. The highest possible ASCII character
        ' in a password is the tilde (~), which has ASCII value 126, so the highest possible value after the
        ' multiplication would be 252.
        Dim encryptionKey(7) As Byte
        Dim index As Integer = 0
        Do While index < encryptionKey.Length AndAlso index < textToEncrypt.Length
            Dim shiftedCharacter As Integer = Convert.ToInt32(textToEncrypt.Chars(index))
            encryptionKey(index) = CByte(shiftedCharacter << 1)
            index += 1
        Loop

        Dim schedule() As UInteger = SetDESKey(encryptionKey)
        Dim singleOutputKey() As UInteger = Body(schedule, firstSaltTranslator, secondSaltTranslator)

        Dim binaryBuffer(8) As Byte
        IntToFourBytes(singleOutputKey(0), binaryBuffer, 0)
        IntToFourBytes(singleOutputKey(1), binaryBuffer, 4)
        binaryBuffer(8) = 0

        Dim binaryBufferIndex As UInteger = 0
        Dim passwordCharacter As UInteger
        Dim bitChecker As UInteger = &H80
        Dim isAnyBitSet, bitCheckerOverflow As Boolean

        For i As Integer = 2 To 12
            passwordCharacter = 0
            For secondIndex As Integer = 0 To 5
                passwordCharacter <<= 1
                isAnyBitSet = ((binaryBuffer(binaryBufferIndex) And bitChecker) <> 0)
                If isAnyBitSet Then
                    passwordCharacter = passwordCharacter Or 1
                End If

                bitChecker >>= 1
                bitCheckerOverflow = (bitChecker = 0)
                If bitCheckerOverflow Then
                    binaryBufferIndex += 1
                    bitChecker = &H80
                End If

                ' The original source had the line below, I moved it outside the compound signs, because it will overwrite the value
                ' a few times before incrementing the index. Where it is now it will be written only once.
                ' Just to be on the safe side, I keep the original line here, so I know where it originally was.
                'encryptionBuffer[index] = Convert.ToChar(m_characterConversionTable[passwordCharacter]);
            Next secondIndex

            encryptionBuffer(i) = Convert.ToChar(m_characterConversionTable(passwordCharacter))
        Next i

        Return encryptionBuffer.ToString()
    End Function
End Class
#End If
