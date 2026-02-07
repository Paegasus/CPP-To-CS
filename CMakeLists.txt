cmake_minimum_required(VERSION 3.16)
project(ui LANGUAGES CXX)

set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

add_executable(ui)

target_include_directories(ui PRIVATE include)

add_subdirectory(src)
