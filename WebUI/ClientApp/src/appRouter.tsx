import React from "react"
import { Route, Switch } from "react-router-dom"
import AffiliateDetails from "./pages/affiliates/affiliate-details"
import { AffiliateList } from "./pages/affiliates/affiliate-list"
import BannerList from "./pages/banners/banner-list"
import { CategoryList } from "./pages/categories/category-list"
import CommentList from "./pages/comments/comment-list"
import { Dashboard } from "./pages/dashboard"
import GameList from "./pages/games/game-list"
import GameItemList from "./pages/games/items/game-item-list"
import { PostList } from "./pages/posts/post-list"
import PostSetting from "./pages/posts/post-setting"
import RoleList from "./pages/roles/role-list"
import Profile from "./pages/users/profile"
import { UserEdit, UserList } from "./pages/users/user-type"

const _preFix = "/admin";
const routes = [
    {
      path: `${_preFix}`,
      exact: true,
      main: () => <Dashboard />
    },
    {
      path: `${_preFix}/comment/list`,
      main: () => <CommentList />
    },
    {
      path: `${_preFix}/banner/list`,
      main: () => <BannerList />
    },
    {
      path: `${_preFix}/user/edit/:id`,
      main: () => <UserEdit />
    },
    {
      path: `${_preFix}/user/list`,
      main: () => <UserList />
    },
    {
      path: `${_preFix}/user/profile`,
      main: () => <Profile />
    },
    {
      path: `${_preFix}/role/list`,
      main: () => <RoleList />
    },
    {
      path: `${_preFix}/affiliate/list`,
      main: () => <AffiliateList />
    },
    {
      path: `${_preFix}/affiliate/details/:id`,
      main: () => <AffiliateDetails />
    },
    {
      path: `${_preFix}/post/setting/:id?`,
      main: () => <PostSetting />
    },
    {
      path: `${_preFix}/post/list`,
      main: () => <PostList />
    },
    {
      path: `${_preFix}/category/list`,
      main: () => <CategoryList />
    },
    {
        path: `${_preFix}/game/list`,
        main: () => <GameList />
    },
    {
        path: `${_preFix}/game/item/:id`,
        main: () => <GameItemList />
    }
  ];

export const AppRouter = () => {
    return (
        <Switch>
            {routes.map((route, index) => (
              <Route key={index} path={route.path} exact={route.exact} children={<route.main />} />
            ))}
        </Switch>
    )
}