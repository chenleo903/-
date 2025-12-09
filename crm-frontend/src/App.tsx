import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { ConfigProvider } from 'antd';
import zhCN from 'antd/locale/zh_CN';
import './App.css';

// Create a client
const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 5 * 60 * 1000, // 5 minutes
      retry: 1,
    },
  },
});

// Placeholder component for home page
function HomePage() {
  return (
    <div style={{ padding: '24px', textAlign: 'center' }}>
      <h1>CRM 系统</h1>
      <p>欢迎使用客户关系管理系统</p>
    </div>
  );
}

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <ConfigProvider locale={zhCN}>
        <BrowserRouter>
          <Routes>
            <Route path="/" element={<HomePage />} />
            {/* Routes will be added in Task 16 */}
          </Routes>
        </BrowserRouter>
      </ConfigProvider>
    </QueryClientProvider>
  );
}

export default App;
