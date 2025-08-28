cmake_minimum_required(VERSION 3.22)
project(GameInputWrapper)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# Windows SDK Include
include_directories("C:/Program Files (x86)/Windows Kits/10/Include/10.0.26100.0/um")
link_directories("C:/Program Files (x86)/Windows Kits/10/Lib/10.0.26100.0/um/x64")

add_compile_options("$<$<CXX_COMPILER_ID:MSVC>:/source-charset:utf-8>")

add_library(GameInputWrapper SHARED GameInputWrapper.cpp)
target_link_libraries(GameInputWrapper gameinput.lib)