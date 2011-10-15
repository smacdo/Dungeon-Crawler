#include "common/platform.h"

std::wstring WinNTStringToWideString( const std::string& str )
{
    size_t slen = str.length();
    int len     = MultiByteToWideChar( CP_ACP, 0,
                                       input.c_str(),
                                       static_cast<int>(slen) + 1,
                                       0,
                                       0 );

    // Allocate space for the new wstring
    wchar_t *buffer = new wchar_t[len];

    MultiByteToWideChar( CP_ACP, 0,
                         str.c_str(),
                         static_cast<int>(slen) + 1,
                         buffer,
                         len );

    std::wstring result( buffer );
    delete[] buffer;

    return result;
}
                                       
