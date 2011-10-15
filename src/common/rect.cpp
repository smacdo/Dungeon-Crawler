#include "rect.h"
#include <ostream>

std::ostream& operator << ( std::ostream& os, const Rect& rect )
{
    return os << "<top: " << rect.x() << ", " << rect.y() << "; w: "
              << rect.width() << "; h: " << rect.height() << ">";
}
